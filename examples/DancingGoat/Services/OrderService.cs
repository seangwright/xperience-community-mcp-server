using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CMS.Commerce;
using CMS.ContentEngine;
using CMS.DataEngine;

using DancingGoat.Commerce;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc.Routing;

#pragma warning disable KXE0002 // Commerce feature is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
namespace DancingGoat.Services;

/// <summary>
/// Service for managing orders.
/// </summary>
internal sealed class OrderService : IOrderService
{
    private readonly IPreferredLanguageRetriever currentLanguageRetriever;
    private readonly IProductVariantsExtractor productVariantsExtractor;
    private readonly ProductRepository productRepository;
    private readonly ProductNameProvider productNameProvider;
    private readonly IInfoProvider<OrderInfo> orderInfoProvider;
    private readonly IInfoProvider<OrderItemInfo> orderItemInfoProvider;
    private readonly IInfoProvider<OrderAddressInfo> orderAddressInfoProvider;
    private readonly IInfoProvider<CustomerInfo> customerInfoProvider;
    private readonly IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider;
    private readonly ICustomerDataRetriever customerDataRetriever;
    private readonly IOrderNumberGenerator orderNumberGenerator;


    public OrderService(
        IPreferredLanguageRetriever currentLanguageRetriever,
        IProductVariantsExtractor productVariantsExtractor,
        ProductRepository productRepository,
        ProductNameProvider productNameProvider,
        IInfoProvider<OrderInfo> orderInfoProvider,
        IInfoProvider<OrderItemInfo> orderItemInfoProvider,
        IInfoProvider<OrderAddressInfo> orderAddressInfoProvider,
        IInfoProvider<CustomerInfo> customerInfoProvider,
        IInfoProvider<CustomerAddressInfo> customerAddressInfoProvider,
        ICustomerDataRetriever customerDataRetriever,
        IOrderNumberGenerator orderNumberGenerator)
    {
        this.currentLanguageRetriever = currentLanguageRetriever;
        this.productVariantsExtractor = productVariantsExtractor;
        this.productRepository = productRepository;
        this.productNameProvider = productNameProvider;
        this.orderInfoProvider = orderInfoProvider;
        this.orderAddressInfoProvider = orderAddressInfoProvider;
        this.customerInfoProvider = customerInfoProvider;
        this.customerAddressInfoProvider = customerAddressInfoProvider;
        this.customerDataRetriever = customerDataRetriever;
        this.orderNumberGenerator = orderNumberGenerator;
        this.orderItemInfoProvider = orderItemInfoProvider;
    }


    /// <summary>
    /// Creates an order based on the provided shopping cart and customer information.
    /// </summary>
    public async Task CreateOrder(ShoppingCartDataModel shoppingCartData, CustomerDto customerDto, int memberId, CancellationToken cancellationToken)
    {
        var languageName = currentLanguageRetriever.Get();

        var products = await productRepository.GetProducts(shoppingCartData.Items.Select(item => item.ContentItemId).ToList(), languageName, cancellationToken);

        var totalPrice = CalculationService.CalculateTotalPrice(shoppingCartData, products);

        using (var scope = new CMSTransactionScope())
        {
            int customerId = await UpsertCustomer(customerDto, memberId, cancellationToken);

            var orderNumber = await orderNumberGenerator.GenerateOrderNumber(cancellationToken);

            var order = new OrderInfo()
            {
                OrderCreatedWhen = DateTime.Now,
                OrderNumber = orderNumber,
                OrderStatus = OrderStatusService.GetInitialStatus(),
                OrderTotalPrice = totalPrice,
                OrderTotalTax = 0,
                OrderTotalShipping = 0,
                OrderGrandTotal = totalPrice,
                OrderCustomerID = customerId
            };
            await orderInfoProvider.SetAsync(order);

            var orderAddress = new OrderAddressInfo()
            {
                OrderAddressFirstName = customerDto.FirstName,
                OrderAddressLastName = customerDto.LastName,
                OrderAddressCompany = customerDto.Company,
                OrderAddressPhone = customerDto.PhoneNumber,
                OrderAddressEmail = customerDto.Email,
                OrderAddressCity = customerDto.AddressCity,
                OrderAddressLine1 = customerDto.AddressLine1,
                OrderAddressLine2 = customerDto.AddressLine2,
                OrderAddressZip = customerDto.AddressPostalCode,
                OrderAddressCountryID = customerDto.AddressCountryId,
                OrderAddressStateID = customerDto.AddressStateId,
                OrderAddressOrderID = order.OrderID,
                OrderAddressType = "Billing",
            };
            await orderAddressInfoProvider.SetAsync(orderAddress);

            foreach (var item in shoppingCartData.Items)
            {
                var product = products.First(product => (product as IContentItemFieldsSource).SystemFields.ContentItemID == item.ContentItemId);
                var variantSKUs = product == null ? null : productVariantsExtractor.ExtractVariantsSKUCode(product);
                var variantSKU = variantSKUs == null || !item.VariantId.HasValue ? null : variantSKUs[item.VariantId.Value];
                var productName = productNameProvider.GetProductName(product, item.VariantId);

                var unitPrice = product.ProductFieldPrice;
                var orderItem = new OrderItemInfo()
                {
                    OrderItemOrderID = order.OrderID,
                    OrderItemUnitCount = item.Quantity,
                    OrderItemUnitPrice = unitPrice,
                    OrderItemTotalPrice = CalculationService.CalculateItemPrice(item.Quantity, unitPrice),
                    OrderItemSKU = variantSKU ?? (product as IProductSKU).ProductSKUCode,
                    OrderItemName = productName
                };
                await orderItemInfoProvider.SetAsync(orderItem);
            }

            scope.Commit();
        }
    }


    /// <summary>
    /// Updates or creates a customer and their address based on the provided data.
    /// </summary>
    private async Task<int> UpsertCustomer(CustomerDto customerDto, int memberId, CancellationToken cancellation)
    {
        CustomerInfo customer = null;

        if (memberId > 0)
        {
            customer = await customerDataRetriever.GetCustomerForMember(memberId, cancellation);
        }

        if (customer == null)
        {
            // Create a new customer if it doesn't exist for the member yet or if the member is not authenticated
            customer = new CustomerInfo()
            {
                CustomerCreatedWhen = DateTime.Now,
                CustomerMemberID = memberId
            };
        }

        // Update the customer with the data from the checkout form
        customer.CustomerFirstName = customerDto.FirstName;
        customer.CustomerLastName = customerDto.LastName;
        customer.CustomerEmail = customerDto.Email;
        customer.CustomerPhone = customerDto.PhoneNumber;

        await customerInfoProvider.SetAsync(customer);

        // Do not cancel the request while a write operation is already in process
        var customerAddress = await customerDataRetriever.GetCustomerAddress(customer.CustomerID, CancellationToken.None);
        if (customerAddress == null)
        {
            customerAddress = new CustomerAddressInfo()
            {
                CustomerAddressCustomerID = customer.CustomerID
            };
        }

        // Update the customer address with the data from the checkout form
        // (Dancing Goat sample operates only with a single customer address)
        customerAddress.CustomerAddressFirstName = customerDto.FirstName;
        customerAddress.CustomerAddressLastName = customerDto.LastName;
        customerAddress.CustomerAddressCompany = customerDto.Company;
        customerAddress.CustomerAddressEmail = customerDto.Email;
        customerAddress.CustomerAddressPhone = customerDto.PhoneNumber;
        customerAddress.CustomerAddressLine1 = customerDto.AddressLine1;
        customerAddress.CustomerAddressLine2 = customerDto.AddressLine2;
        customerAddress.CustomerAddressCity = customerDto.AddressCity;
        customerAddress.CustomerAddressZip = customerDto.AddressPostalCode;
        customerAddress.CustomerAddressCountryID = customerDto.AddressCountryId;
        customerAddress.CustomerAddressStateID = customerDto.AddressStateId;

        await customerAddressInfoProvider.SetAsync(customerAddress);

        return customer.CustomerID;
    }
}
#pragma warning restore KXE0002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
