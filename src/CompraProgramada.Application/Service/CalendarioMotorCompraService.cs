using CompraProgramada.Application.Config;
using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Service;

public class CalendarioMotorCompraService : ICalendarioMotorCompraService
{
    private readonly MotorCompraConfig _config;
    private readonly IDateTimeProvaider _dateTimeProvaider;
    private DateTime _dataAtual => _dateTimeProvaider.Now;

    public CalendarioMotorCompraService(AppConfig config)
        : this(config, new DateTimeProvaider()) { }

    public CalendarioMotorCompraService(AppConfig config,
        IDateTimeProvaider dateTimeProvaider)
    {
        _config = config.MotorCompraConfig;
        _dateTimeProvaider = dateTimeProvaider;
    }

    public bool DeveExecutarCompraHoje()
    {
        var ehDiaUtil = EhDiaUtil(_dataAtual);

        if (_config.DiasDeCompra.Contains(_dataAtual.Day) && ehDiaUtil)
            return true;

        return false;
    }

    public DateTime ObterProximaDataCompra()
    {
        DateTime? proximaDataCompra;
        var diasOrdenadosDeExecucao = _config.DiasDeCompra.OrderBy(d => d).ToList();

        foreach (var dia in diasOrdenadosDeExecucao)
        {
            if (dia > _dataAtual.Day)
            {
                proximaDataCompra = new DateTime(_dataAtual.Year, _dataAtual.Month, dia);

                if (EhDiaUtil(proximaDataCompra.Value))
                    return proximaDataCompra.Value;

                return ObterProximoDiaUtil(proximaDataCompra.Value);
            }
        }

        DateTime proximoMes = _dataAtual.AddMonths(1);

        proximaDataCompra = new DateTime(proximoMes.Year, proximoMes.Month, diasOrdenadosDeExecucao.First());

        if (EhDiaUtil(proximaDataCompra.Value))
            return proximaDataCompra.Value;

        return ObterProximoDiaUtil(proximaDataCompra.Value);
    }

    public DateTime ObterDataReferenciaExecucao(DateTime dataExecutada)
    {
        if (_config.DiasDeCompra.Contains(dataExecutada.Day))
            return dataExecutada;

        var diasOrdenadosDeExecucao = _config.DiasDeCompra.OrderBy(d => d).ToList();

        foreach (var dia in diasOrdenadosDeExecucao)
        {
            if (dataExecutada.Day > 0 && dataExecutada.Day < diasOrdenadosDeExecucao.First())
            {
                var mesAnterior = _dataAtual.AddMonths(-1);
                return new DateTime(mesAnterior.Year, mesAnterior.Month, diasOrdenadosDeExecucao[diasOrdenadosDeExecucao.Count - 1]);
            }

            if (dataExecutada.Day < dia)
            {
                var indexDia = diasOrdenadosDeExecucao.IndexOf(dia);
                return new DateTime(_dataAtual.Year, _dataAtual.Month, diasOrdenadosDeExecucao[indexDia - 1]);
            }
        }

        return new DateTime(_dataAtual.Year, _dataAtual.Month, diasOrdenadosDeExecucao[diasOrdenadosDeExecucao.Count - 1]);
    }

    public bool EhDiaUtil(DateTime data)
        => data.DayOfWeek != DayOfWeek.Saturday &&
            data.DayOfWeek != DayOfWeek.Sunday;

    public DateTime ObterProximoDiaUtil(DateTime data)
        => data.DayOfWeek == DayOfWeek.Saturday ?
            data.AddDays(2) : data.DayOfWeek == DayOfWeek.Sunday ?
            data.AddDays(1) : data;
}