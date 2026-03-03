using CompraProgramada.Application.Interface;
using CompraProgramada.Application.Request;
using CompraProgramada.Application.Response;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Application.Handler;

public class ClienteHandle
    : IRequestHandler<AdesaoRequest, Result<AdesaoResponse>>
{
    private readonly ILogger<ClienteHandle> _logger;
    private readonly IClienteService _clienteService;

    public ClienteHandle(ILogger<ClienteHandle> logger,
        IClienteService clienteService)
    {
        _logger = logger;
        _clienteService = clienteService;
    }

    public async Task<Result<AdesaoResponse>> Handle(AdesaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de adesão para o cliente {Nome} com CPF {Cpf}.", request.Nome, request.Cpf);

        var result = await _clienteService.CadastrarClienteAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao processar adesão: {Exception}", result.Exception);
            return Result<AdesaoResponse>.Fail(result.Exception);
        }

        var cliente = result.Value!;
        var contaGraficaCliente = cliente.ContaGrafica!;

        return Result<AdesaoResponse>.Ok(new AdesaoResponse
        {
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Cpf = cliente.Cpf,
            Email = cliente.Email,
            ValorMensal = cliente.ValorMensal,
            Ativo = cliente.Ativo,
            DataAdesao = cliente.DataAdesao,
            ContaGrafica = new (contaGraficaCliente.Id, contaGraficaCliente.NumeroConta, contaGraficaCliente.Tipo, contaGraficaCliente.DataCriacao)
        });
    }
}