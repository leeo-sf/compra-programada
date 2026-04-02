using CompraProgramada.Infra.Converter;
using System.Text.Json;

namespace CompraProgramada.Infra.Tests.Converter;

public class UtcDateTimeConverterTests
{
    private readonly UtcDateTimeConverter _sut;

    public UtcDateTimeConverterTests() => _sut = new();

    [Fact]
    public void Read_DeveConverterParaUtc_QuandoDataValida()
    {
        // Arrange
        var json = "\"2026-04-10T13:45:23\"";
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
        reader.Read();

        // Act
        var result = _sut.Read(ref reader, typeof(DateTime), new JsonSerializerOptions());

        // Assert
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(DateTime.Parse("2026-04-10T13:45:23").ToUniversalTime(), result);
    }

    [Fact]
    public void Read_DeveLancarErro_QuandoDataInvalida()
    {
        // Arrange
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UtcDateTimeConverter());

        var json = "\"data-invalida\"";

        // Act & Assert
        Assert.Throws<FormatException>(() =>
            JsonSerializer.Deserialize<DateTime>(json, options));
    }

    [Fact]
    public void Write_DeveEscreverDataEmUtc_NoFormatoCorreto()
    {
        // Arrange
        var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream);

        var data = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local);

        // Act
        _sut.Write(writer, data, new JsonSerializerOptions());
        writer.Flush();

        var json = System.Text.Encoding.UTF8.GetString(stream.ToArray());

        // Assert
        Assert.Equal($"\"{data.ToUniversalTime():yyyy-MM-ddTHH:mm:ssZ}\"", json);
    }
}