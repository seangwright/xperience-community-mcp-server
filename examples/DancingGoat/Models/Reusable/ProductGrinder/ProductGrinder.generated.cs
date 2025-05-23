//--------------------------------------------------------------------------------------------------
// <auto-generated>
//
//     This code was generated by code generator tool.
//
//     To customize the code use your own partial class. For more info about how to use and customize
//     the generated code see the documentation at https://docs.xperience.io/.
//
// </auto-generated>
//--------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CMS.ContentEngine;

namespace DancingGoat.Models
{
	/// <summary>
	/// Represents a content item of type <see cref="ProductGrinder"/>.
	/// </summary>
	[RegisterContentTypeMapping(CONTENT_TYPE_NAME)]
	public partial class ProductGrinder : IContentItemFieldsSource, IProductFields, IProductSKU, IProductManufacturer
	{
		/// <summary>
		/// Code name of the content type.
		/// </summary>
		public const string CONTENT_TYPE_NAME = "DancingGoat.ProductGrinder";


		/// <summary>
		/// Represents system properties for a content item.
		/// </summary>
		[SystemField]
		public ContentItemFields SystemFields { get; set; }


		/// <summary>
		/// GrinderType.
		/// </summary>
		public string GrinderType { get; set; }


		/// <summary>
		/// GrinderPower.
		/// </summary>
		public decimal GrinderPower { get; set; }


		/// <summary>
		/// ProductFieldName.
		/// </summary>
		public string ProductFieldName { get; set; }


		/// <summary>
		/// ProductFieldDescription.
		/// </summary>
		public string ProductFieldDescription { get; set; }


		/// <summary>
		/// ProductFieldImage.
		/// </summary>
		public IEnumerable<Image> ProductFieldImage { get; set; }


		/// <summary>
		/// ProductFieldPrice.
		/// </summary>
		public decimal ProductFieldPrice { get; set; }


		/// <summary>
		/// ProductFieldTags.
		/// </summary>
		public IEnumerable<TagReference> ProductFieldTags { get; set; }


		/// <summary>
		/// ProductFieldCategory.
		/// </summary>
		public IEnumerable<TagReference> ProductFieldCategory { get; set; }


		/// <summary>
		/// ProductSKUCode.
		/// </summary>
		public string ProductSKUCode { get; set; }


		/// <summary>
		/// ProductManufacturerTag.
		/// </summary>
		public IEnumerable<TagReference> ProductManufacturerTag { get; set; }
	}
}