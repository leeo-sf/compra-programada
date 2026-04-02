using CompraProgramada.Shared.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;
using CompraProgramada.Shared.Dto;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class ClienteMapper
{
    private readonly ContaMapper _contaMapper;

    public ClienteMapper(ContaMapper contaMapper) => _contaMapper = contaMapper;


    [MapProperty(nameof(Cliente.Id), nameof(ClienteDto.ClienteId))]
    public partial ClienteDto ToResponse(Cliente cliente);
    public partial List<ClienteDto> ToResponse(List<Cliente> cliente);


    [MapProperty(nameof(Cliente.Id), nameof(AdesaoResponse.ClienteId))]
    public partial AdesaoResponse ToAdesaoResponse(Cliente cliente);


    [MapProperty(nameof(Cliente.Id), nameof(SaidaProdutoResponse.ClienteId))]
    public partial SaidaProdutoResponse ToSaidaProdutoResponse(Cliente cliente);


    [MapProperty(nameof(Cliente.Id), nameof(AtualizarValorMensalResponse.ClienteId))]
    [MapProperty(nameof(Cliente.ValorAnterior), nameof(AtualizarValorMensalResponse.ValorMensalAnterior))]
    [MapProperty(nameof(Cliente.ValorMensal), nameof(AtualizarValorMensalResponse.ValorMensalNovo))]
    public partial AtualizarValorMensalResponse ToAtualizarValorMensalResponse(Cliente cliente);


    public ContaGraficaDto ToResponse(ContaGrafica conta) => _contaMapper.ToResponse(conta);
    public ContaGraficaResponse ToContaGraficaResponse(ContaGrafica conta) => _contaMapper.ToContaGraficaResponse(conta);
}