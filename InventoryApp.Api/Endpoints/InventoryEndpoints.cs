using InventoryApp.Api.Data;
using InventoryApp.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryApp.Api.Endpoints;

public static class InventoryEndpoints
{
    public static RouteGroupBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapGet("/", GetAll)
            .WithName("GetInventory")
            .WithSummary("List all inventory items");

        group.MapGet("/{id:int}", GetById)
            .WithName("GetInventoryItem")
            .WithSummary("Get one inventory item")
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id:int}", UpdateQuantity)
            .WithName("UpdateInventoryQuantity")
            .WithSummary("Set the on-hand quantity of an item")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<Ok<IReadOnlyList<InventoryItem>>> GetAll(
        IInventoryStore store, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await store.GetAllAsync(cancellationToken));
    }

    private static async Task<Results<Ok<InventoryItem>, ProblemHttpResult>> GetById(
        int id, IInventoryStore store, CancellationToken cancellationToken)
    {
        var item = await store.GetByIdAsync(id, cancellationToken);
        return item is null ? ItemNotFound(id) : TypedResults.Ok(item);
    }

    private static async Task<Results<Ok<InventoryItem>, ValidationProblem, ProblemHttpResult>> UpdateQuantity(
        int id, UpdateQuantityRequest request, IInventoryStore store, CancellationToken cancellationToken)
    {
        if (request.CurrentQuantity < 0)
        {
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    ["currentQuantity"] = ["Quantity cannot be negative."]
                },
                title: "Invalid quantity");
        }

        var updated = await store.UpdateQuantityAsync(id, request.CurrentQuantity, cancellationToken);
        return updated is null ? ItemNotFound(id) : TypedResults.Ok(updated);
    }

    private static ProblemHttpResult ItemNotFound(int id) =>
        TypedResults.Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Inventory item not found",
            detail: $"No inventory item exists with id {id}.");
}
