using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Domain.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class DistribuicaoMapper
{
    [MapProperty("ContaGrafica.Cliente.Cpf", nameof(DistribuicaoDto.Cpf))]
    [MapProperty("OrdemCompra.Data", nameof(DistribuicaoDto.Data))]
    [MapProperty("ContaGrafica.ClienteId", nameof(DistribuicaoDto.ClienteId))]
    [MapProperty("ContaGrafica.Cliente.Nome", nameof(DistribuicaoDto.Nome))]
    [MapProperty("ContaGrafica.Cliente.ValorAporte", nameof(DistribuicaoDto.ValorAporte))]
    [MapProperty(nameof(Distribuicao), nameof(DistribuicaoDto.Ativos))]
    public partial DistribuicaoDto ToResponse(Distribuicao distribuicao);
    public partial List<DistribuicaoDto> ToResponse(List<Distribuicao> distribuicoes);
}