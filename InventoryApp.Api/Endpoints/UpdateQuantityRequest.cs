namespace InventoryApp.Api.Endpoints;

public sealed class UpdateQuantityRequest
{
    public required int CurrentQuantity { get; init; }
}
