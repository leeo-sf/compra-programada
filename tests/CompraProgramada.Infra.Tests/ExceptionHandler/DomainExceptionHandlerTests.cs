using CompraProgramada.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using CompraProgramada.Shared.Exceptions.Base;

namespace CompraProgramada.Infra.Tests.ExceptionHandler;

public class DomainExceptionHandlerTests
{
    [Theory]
    [MemberData(nameof(DomainExceptions))]
    public async Task Deve_Tratar_DomainException_Quando_ExceptionsTratadas(DomainException exception)
    {
        // Arrange
        var logger = NullLogger<DomainExceptionHandler>.Instance;
        var handler = new DomainExceptionHandler(logger);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        Assert.True(handled);
        Assert.Equal(context.Response.StatusCode, (int)exception.StatusCode);
    }

    [Fact]
    public async Task Deve_Nao_Tratar_DomainException_Quando_ExceptionsNaoTratadas()
    {
        // Arrange
        var logger = NullLogger<DomainExceptionHandler>.Instance;
        var handler = new DomainExceptionHandler(logger);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var exception = new ApplicationException("Test");

        // Act
        var handled = await handler.TryHandleAsync(context, exception, default);

        // Assert
        Assert.False(handled);
    }

    public static TheoryData<DomainException> DomainExceptions()
        => new()
        {
            new DomainException("Test", "Test code", HttpStatusCode.InternalServerError),
            new ClienteNaoEncontradoException(),
            new CompraException("Message", "Code", HttpStatusCode.UnprocessableEntity),
            new CpfExistenteException(),
            new PercentualCestaException(98),
            new QuantidadeItensCestaException(2),
            new QuantidadeNegativaException(),
            new TickerNaoPreenchidoException(),
            new ValorMensalException(10),
            new QuantidadeCustodiaException()
        };
}