using CompraProgramada.Shared.Request;
using CompraProgramada.Shared.Response;
using Microsoft.Extensions.Logging;

namespace CompraProgramada.Shared.Extensions;

public static partial class LogDefinitions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando processo de adesão para o cliente {Nome}")]
    public static partial void LogInicioProcessoAdesao(this ILogger logger, string nome);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Adesão não pode ser realizada, pois não há cesta recomendada ativa")]
    public static partial void LogFalhaAdesaoCestaNaoAtiva(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Adesão realizada com sucesso para o cliente {Nome} com Id {Id}.")]
    public static partial void LogAdesaoRealizadaComSucesso(this ILogger logger, string nome, int id);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando processo de saida do produto para o cliente {ClienteId}.")]
    public static partial void LogInicioProcessoSaidaProduto(this ILogger logger, int clienteId);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Cliente {ClienteId} já está inativo.")]
    public static partial void LogClienteInativo(this ILogger logger, int clienteId);
    [LoggerMessage(Level = LogLevel.Information, Message = "Solicitação de saída do produto realizada com sucesso.")]
    public static partial void LogSaidaProdutoRealizadaComSucesso(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Solicitação do ClienteId: {ClienteId} para alteração do valor mensal, novo valor mensal: {NovoValorMensal}")]
    public static partial void LogInicioAtualizacaoValorMensal(this ILogger logger, int clienteId, decimal novoValorMensal);
    [LoggerMessage(Level = LogLevel.Information, Message = "Valor mensal do ClienteId {ClientId} atualizado para: {NovoValor}")]
    public static partial void LogValorMensalAtualizado(this ILogger logger, int clientId, decimal novoValor);
    [LoggerMessage(Level = LogLevel.Information, Message = "Cliente solicitando consulta da carteira: {ClienteId}")]
    public static partial void LogConsultaCarteira(this ILogger logger, int clienteId);
    [LoggerMessage(Level = LogLevel.Information, Message = "Cliente solicitando consulta da rentabilidade da carteira: {ClienteId}")]
    public static partial void LogConsultaRentabilidade(this ILogger logger, int clienteId);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando consulta da cesta atual.")]
    public static partial void LogConsultaCestaAtual(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando processo de consulta de histórico de cestas.")]
    public static partial void LogConsultaCestaHistorico(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando processo de criação/atualização de cesta: {Request}")]
    public static partial void LogInicioCriacaoOuAtualizacaoCesta(this ILogger logger, CriarCestaRecomendadaRequest request);
    [LoggerMessage(Level = LogLevel.Information, Message = "Cesta registrada na base de dados Nome: {Nome} > Composição: {ComposicaoCesta}")]
    public static partial void LogCestaRegistrada(this ILogger logger, string nome, object composicaoCesta);
    [LoggerMessage(Level = LogLevel.Information, Message = "Cesta atual desativada Nome: {Nome} > Composição: {ComposicaoCesta}")]
    public static partial void LogCestaDesativada(this ILogger logger, string nome, object composicaoCesta);
    [LoggerMessage(Level = LogLevel.Warning, Message = "MotorCompra não será executado hoje. Próxima data de compra prevista para {DataProximaExecucao}. Encerrando processo.")]
    public static partial void LogAvisoExecucaoMotorCompra(this ILogger logger, DateTime dataProximaExecucao);
    [LoggerMessage(Level = LogLevel.Information, Message = "{QuantidadeClientes} clientes ativos para processamento.")]
    public static partial void LogQuantidadeClientesAtivosParaDistribuicao(this ILogger logger, int quantidadeClientes);
    [LoggerMessage(Level = LogLevel.Information, Message = "Total Consolidado a ser comprado: {TotalConsolidado}")]
    public static partial void LogTotalConsolidadoParaCompra(this ILogger logger, decimal totalConsolidado);
    [LoggerMessage(Level = LogLevel.Information, Message = "Distribuições para as custodias realizadas.")]
    public static partial void LogDistribuicoesRealizadas(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Imposto de renda calculado e publicado para {QtdClientes} clientes.")]
    public static partial void LogIrPublicado(this ILogger logger, int qtdClientes);
    [LoggerMessage(Level = LogLevel.Information, Message = "Registrado histórico da execução do motor de compra na base de dados.")]
    public static partial void LogRegitroHistoricoExecucaoMotorCompra(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Resíduos não distribuídos obtidos para reaproveitamento na distribuição")]
    public static partial void LogResiduosParaDistribuicao(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Atualização realizada das nas contas que tiveram a distribuição na base de dados.")]
    public static partial void LogAtualizacaoContasDistribuicao(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Cotações de fachamento B3 da cesta Top Five com base na data pregão {DataPregao}.")]
    public static partial void LogCotacoesFachamentoB3(this ILogger logger, DateOnly dataPregao);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando emissão de ordens de compra...")]
    public static partial void LogIniciandoEmissaoOrdensCompra(this ILogger logger);
    [LoggerMessage(Level = LogLevel.Information, Message = "Fechamento dos ativos correspondentes a cesta atual obtidos: {Fechamentos}")]
    public static partial void LogFechamentosObtidos(this ILogger logger, object fechamentos);
    [LoggerMessage(Level = LogLevel.Warning, Message = "Ativo {Ticker} não encontrado na composição da cesta vigente")]
    public static partial void LogAtivoNaoEncontrado(this ILogger logger, string ticker);
    [LoggerMessage(Level = LogLevel.Information, Message = "Ordens de compra emitidas e registradas. {OrdemCompra}")]
    public static partial void LogOrdensCompraEmitidas(this ILogger logger, object ordemCompra);
    [LoggerMessage(Level = LogLevel.Error, Message = "Erro ao executar o motor de compra.")]
    public static partial void LogErroExecucaoMotorCompra(this ILogger logger, Exception exception);
    [LoggerMessage(Level = LogLevel.Information, Message = "Motor de compra executado com sucesso {Result}.")]
    public static partial void LogSucessoExecucaoMotorCompra(this ILogger logger, ExecutarMotorCompraResponse result);
    [LoggerMessage(Level = LogLevel.Information, Message = "Iniciando o motor de compra...")]
    public static partial void LogIniciandoMotorCompra(this ILogger logger);
}