using CompraProgramada.Shared.Dto;
using CompraProgramada.Domain.Entity;
using Riok.Mapperly.Abstractions;

namespace CompraProgramada.Application.Mapper;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class DistribuicaoMapper
{
    private readonly ContaMapper _contaMapper;
    private readonly HistoricoCompraMapper _historicoCompraMapper;
    private readonly CustodiaFilhoteMapper _custodiaFilhoteMapper;

    public DistribuicaoMapper(ContaMapper contaMapper,
        HistoricoCompraMapper historicoCompraMapper,
        CustodiaFilhoteMapper custodiaFilhoteMapper)
    {
        _contaMapper = contaMapper;
        _historicoCompraMapper = historicoCompraMapper;
        _custodiaFilhoteMapper = custodiaFilhoteMapper;
    }

    [MapProperty("ContaGrafica.Cliente.Cpf", nameof(DistribuicaoDto.Cpf))]
    [MapProperty("OrdemCompra.Data", nameof(DistribuicaoDto.Data))]
    [MapProperty("ContaGrafica.ClienteId", nameof(DistribuicaoDto.ClienteId))]
    [MapProperty("ContaGrafica.Cliente.Nome", nameof(DistribuicaoDto.Nome))]
    [MapProperty("ContaGrafica.Cliente.ValorAporte", nameof(DistribuicaoDto.ValorAporte))]
    [MapProperty(nameof(Distribuicao), nameof(DistribuicaoDto.Ativos))]
    public partial DistribuicaoDto ToResponse(Distribuicao distribuicao);
    public partial List<DistribuicaoDto> ToResponse(List<Distribuicao> distribuicoes);

    public ContaGraficaDto ToResponse(ContaGrafica conta) => _contaMapper.ToResponse(conta);
    public List<ContaGraficaDto> ToResponse(List<ContaGrafica> contas) => _contaMapper.ToResponse(contas);

    public HistoricoCompraDto ToResponse(HistoricoCompra historico) => _historicoCompraMapper.ToResponse(historico);
    public List<HistoricoCompraDto> ToResponse(List<HistoricoCompra> historico) => _historicoCompraMapper.ToResponse(historico);

    public CustodiaFilhoteDto ToResponse(CustodiaFilhote custodia) => _custodiaFilhoteMapper.ToResponse(custodia);
    public List<CustodiaFilhoteDto> ToResponse(List<CustodiaFilhote> custodias) => _custodiaFilhoteMapper.ToResponse(custodias);

    private List<AtivoQuantidadeDto> MapAtivos(Distribuicao distribuicao)
        => new List<AtivoQuantidadeDto>
        {
            new AtivoQuantidadeDto { Ticker = distribuicao.Ticker, Quantidade = distribuicao.QuantidadeAlocada }
        };
}