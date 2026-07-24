using System.Text.Json;

namespace CompraProgramada.Api.Tests;

public static class TestUtils
{
    public static TResponse? ReadResultContentApi<TResponse>(string responseContent)
        where TResponse : class
    {
        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<TResponse>(responseContent, options);
    }
}

internal record ErroResponse(string Mensagem, string Codigo);