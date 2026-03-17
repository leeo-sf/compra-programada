using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class ClienteMapper
{
    [MapProperty(nameof(Cliente.Id), nameof(ClienteDto.ClienteId))]
    public partial ClienteDto ToResponse(Cliente cliente);
    public partial List<ClienteDto> ToResponse(List<Cliente> cliente);

    public partial AdesaoResponse ToAdesaoResponse(ClienteDto cliente);

    public partial SaidaProdutoResponse ToSaidaProdutoResponse(ClienteDto cliente);

    [MapProperty(nameof(ClienteDto.ValorAnterior), nameof(AtualizarValorMensalResponse.ValorMensalAnterior))]
    [MapProperty(nameof(ClienteDto.ValorMensal), nameof(AtualizarValorMensalResponse.ValorMensalNovo))]
    public partial AtualizarValorMensalResponse ToAtualizarValorMensalResponse(ClienteDto cliente);
}