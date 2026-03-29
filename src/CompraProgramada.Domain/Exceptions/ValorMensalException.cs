using CompraProgramada.Domain.Exceptions.Base;
using System.Globalization;

namespace CompraProgramada.Domain.Exceptions;

public class ValorMensalException : DomainException
{
    public ValorMensalException(decimal valorMinimoAdesao)
        : base($"O valor mensal minimo e de R$ {valorMinimoAdesao.ToString("F2", new CultureInfo("pt-BR"))}",
            "VALOR_MENSAL_INVALIDO")
    { }
}