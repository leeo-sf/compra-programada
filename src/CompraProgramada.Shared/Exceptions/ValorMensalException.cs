using CompraProgramada.Shared.Exceptions.Base;
using System.Globalization;

namespace CompraProgramada.Shared.Exceptions;

public class ValorMensalException : DomainException
{
    public ValorMensalException(decimal valorMinimoAdesao)
        : base($"O valor mensal mínimo é de R$ {valorMinimoAdesao.ToString("F2", new CultureInfo("pt-BR"))}",
            "VALOR_MENSAL_INVALIDO")
    { }
}