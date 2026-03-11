using CompraProgramada.Domain.Exceptions.Base;

namespace CompraProgramada.Domain.Exceptions;

public class ValorMensalException : DomainException
{
    public ValorMensalException(decimal valorMinimoAdesao)
        : base($"O valor mensal minimo e de R$ {valorMinimoAdesao.ToString("F2")}",
            "VALOR_MENSAL_INVALIDO")
    { }
}