namespace DancingGoat.Services;

internal class OrderStatusService
{
    private static readonly string[] STATUSES = new string[] { "Created", "Processing", "Processed", "For pickup", "Picked up", "Finalized" };

    public static string GetInitialStatus()
    {
        return STATUSES[0];
    }
}
