using InventoryApp.Shared.Models;

namespace InventoryApp.Api.Data;

public interface IInventoryStore
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<InventoryItem?> UpdateQuantityAsync(int id, int newQuantity, CancellationToken cancellationToken = default);
}
