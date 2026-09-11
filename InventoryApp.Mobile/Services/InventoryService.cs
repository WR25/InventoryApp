using System.Net.Http.Json;
using InventoryApp.Mobile.Models;
using Microsoft.Extensions.Logging;

namespace InventoryApp.Mobile.Services;

public class InventoryService : IInventoryService
{
    private const string InventoryPath = "api/inventory";

    private readonly HttpClient _http;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(HttpClient http, ILogger<InventoryService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<InventoryItem>?> GetInventoryAsync()
    {
        try
        {
            var items = await _http.GetFromJsonAsync<List<InventoryItem>>(InventoryPath);
            return items ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load inventory from {BaseAddress}", _http.BaseAddress);
            return null;
        }
    }

    public async Task<InventoryItem?> GetItemAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<InventoryItem>($"{InventoryPath}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not load inventory item {ItemId}", id);
            return null;
        }
    }

    public async Task<bool> UpdateQuantityAsync(int id, int newQuantity)
    {
        try
        {
            using var response = await _http.PutAsJsonAsync(
                $"{InventoryPath}/{id}", new { currentQuantity = newQuantity });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Updating item {ItemId} failed with HTTP {StatusCode}",
                    id, (int)response.StatusCode);
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not update quantity of item {ItemId}", id);
            return false;
        }
    }
}
