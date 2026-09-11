using InventoryApp.Shared.Models;

namespace InventoryApp.Api.Data;

public sealed class InMemoryInventoryStore : IInventoryStore
{
    private readonly Lock _gate = new();
    private readonly Dictionary<int, InventoryItem> _items;

    public InMemoryInventoryStore()
    {
        _items = SeedItems().ToDictionary(item => item.ItemId);
    }

    public Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            IReadOnlyList<InventoryItem> snapshot = _items.Values
                .OrderBy(item => item.ItemId)
                .Select(Copy)
                .ToList();
            return Task.FromResult(snapshot);
        }
    }

    public Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            return Task.FromResult(_items.TryGetValue(id, out var item) ? Copy(item) : null);
        }
    }

    public Task<InventoryItem?> UpdateQuantityAsync(int id, int newQuantity, CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (!_items.TryGetValue(id, out var item))
            {
                return Task.FromResult<InventoryItem?>(null);
            }

            item.CurrentQuantity = newQuantity;
            item.LastUpdated = DateTime.UtcNow;
            return Task.FromResult<InventoryItem?>(Copy(item));
        }
    }

    private static InventoryItem Copy(InventoryItem item) => new()
    {
        ItemId = item.ItemId,
        ItemName = item.ItemName,
        CurrentQuantity = item.CurrentQuantity,
        LastUpdated = item.LastUpdated
    };

    private static IEnumerable<InventoryItem> SeedItems()
    {
        var now = DateTime.UtcNow;

        yield return new() { ItemId = 1, ItemName = "Hex Bolt M10 x 40mm, Grade 8.8 Zinc-Plated", CurrentQuantity = 4200, LastUpdated = now.AddHours(-3) };
        yield return new() { ItemId = 2, ItemName = "Deep Groove Ball Bearing 6205-2RS", CurrentQuantity = 340, LastUpdated = now.AddHours(-26) };
        yield return new() { ItemId = 3, ItemName = "Aluminum Bar Stock 6061-T6, 25mm x 3.6m", CurrentQuantity = 85, LastUpdated = now.AddDays(-2) };
        yield return new() { ItemId = 4, ItemName = "Nitrile O-Ring AS568-214, 70 Durometer", CurrentQuantity = 1500, LastUpdated = now.AddHours(-9) };
        yield return new() { ItemId = 5, ItemName = "Hydraulic Hose Assembly 1/2in x 900mm, 3000 PSI", CurrentQuantity = 60, LastUpdated = now.AddDays(-4) };
        yield return new() { ItemId = 6, ItemName = "Carbide End Mill 12mm, 4-Flute TiAlN", CurrentQuantity = 14, LastUpdated = now.AddHours(-1) };
        yield return new() { ItemId = 7, ItemName = "Inductive Proximity Sensor M18, 24V DC PNP", CurrentQuantity = 120, LastUpdated = now.AddDays(-6) };
        yield return new() { ItemId = 8, ItemName = "Classical V-Belt B-Section, 1219mm", CurrentQuantity = 0, LastUpdated = now.AddDays(-1) };
    }
}
