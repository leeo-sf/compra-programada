using CompraProgramada.Application.Contract.Service;
using CompraProgramada.Application.Mapper;
using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using OperationResult;

namespace CompraProgramada.Application.Handler;

public class ClienteHandle
    : IRequestHandler<AdesaoRequest, Result<AdesaoResponse>>,
        IRequestHandler<SaidaProdutoRequest, Result<SaidaProdutoResponse>>,
        IRequestHandler<AtualizarValorMensalRequest, Result<AtualizarValorMensalResponse>>,
        IRequestHandler<CarteiraCustodiaRequest, Result<CarteiraCustodiaResponse>>,
        IRequestHandler<RentabilidadeRequest, Result<RentabilidadeResponse>>
{
    private readonly ILogger<ClienteHandle> _logger;
    private readonly IClienteService _clienteService;
    private readonly ClienteMapper _mapper;

    public ClienteHandle(ILogger<ClienteHandle> logger,
        IClienteService clienteService,
        ClienteMapper mapper)
    {
        _logger = logger;
        _clienteService = clienteService;
        _mapper = mapper;
    }

    public async Task<Result<AdesaoResponse>> Handle(AdesaoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de adesão para o cliente {Nome} com CPF {Cpf}.", request.Nome, request.Cpf);

        var result = await _clienteService.RealizarAdesaoAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Falha ao processar adesão: {Exception}", result.Exception);
            return result.Exception;
        }

        var cliente = result.Value!;

        _logger.LogInformation("Adesão realizada com sucesso para o cliente {Nome} com CPF {Cpf}.", request.Nome, request.Cpf);

        return _mapper.ToAdesaoResponse(cliente);
    }

    public async Task<Result<SaidaProdutoResponse>> Handle(SaidaProdutoRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando processo de saida do produto para o cliente {ClienteId}.", request.ClienteId);

        var result = await _clienteService.SairDoProdutoAsync(request.ClienteId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro na alteração do valor mensal. {Error}", result.Exception);
            return result.Exception;
        }

        _logger.LogInformation("Solicitação de saída do produto realizada com sucesso.");

        return _mapper.ToSaidaProdutoResponse(result.Value);
    }

    public async Task<Result<AtualizarValorMensalResponse>> Handle(AtualizarValorMensalRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Solicitação do ClienteId: {ClienteId} para alteração do valor mensal, novo valor mensal: {NovoValorMensal}", request.ClienteId, request.NovoValorMensal);

        var atualizaValorMensalResult = await _clienteService.AtualizarValorMensalAsync(request, cancellationToken);

        if (!atualizaValorMensalResult.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro na alteração do valor mensal. {Error}", atualizaValorMensalResult.Exception);
            return atualizaValorMensalResult.Exception;
        }

        var cliente = atualizaValorMensalResult.Value;

        _logger.LogInformation("Valor mensal do ClienteId {ClientId} atualizado para: {NovoValor}", cliente.Id, cliente.ValorMensal);

        return _mapper.ToAtualizarValorMensalResponse(cliente);
    }

    public async Task<Result<CarteiraCustodiaResponse>> Handle(CarteiraCustodiaRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cliente solicitando consulta da carteira: {ClienteId}", request);

        var carteiraResult = await _clienteService.ConsultarCarteiraAsync(request.ClienteId, cancellationToken);

        if (!carteiraResult.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro na consulta da carteira. {Error}", carteiraResult.Exception);
            return carteiraResult.Exception;
        }

        return carteiraResult.Value;
    }

    public async Task<Result<RentabilidadeResponse>> Handle(RentabilidadeRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cliente solicitando consulta de rentabilidade da carteira: {ClienteId}", request);

        var carteiraResult = await _clienteService.ConsultarRentabilidadeAsync(request.ClienteId, cancellationToken);

        if (!carteiraResult.IsSuccess)
        {
            _logger.LogError("Ocorreu um erro na consulta da carteira. {Error}", carteiraResult.Exception);
            return carteiraResult.Exception;
        }

        return carteiraResult.Value;
    }
}