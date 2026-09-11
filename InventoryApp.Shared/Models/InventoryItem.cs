namespace InventoryApp.Shared.Models;

public class InventoryItem
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int CurrentQuantity { get; set; }
    public DateTime LastUpdated { get; set; }
}
