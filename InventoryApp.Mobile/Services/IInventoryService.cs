using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Services;

public interface IInventoryService
{
    Task<List<InventoryItem>?> GetInventoryAsync();

    Task<InventoryItem?> GetItemAsync(int id);

    Task<bool> UpdateQuantityAsync(int id, int newQuantity);
}
