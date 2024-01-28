using System.Text.Json;

namespace AuctionApp.Application.ApiResponses;

public class ApiResponse<T>(T? data, string message, bool success)
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public bool Success { get; init; } = success;
    public string Message { get; init; } = message;
    public string Note { get; init; } = "N/A";
    public T? Data { get; init; } = data;
}