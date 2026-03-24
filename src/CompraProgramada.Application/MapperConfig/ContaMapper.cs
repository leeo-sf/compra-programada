using CompraProgramada.Application.Dto;
using CompraProgramada.Application.Response;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class ContaMapper
{
    public partial ContaGraficaDto ToResponse(ContaGrafica contaGrafica);
    public partial List<ContaGraficaDto> ToResponse(List<ContaGrafica> contasGrafica);

    public partial ContaGraficaResponse ToResponse(ContaGraficaDto contaGrafica);

    public partial HistoricoCompraDto ToResponse(HistoricoCompra historicoCompra);

    public partial CustodiaFilhoteDto ToResponse(CustodiaFilhote custodiaFilhote);
}