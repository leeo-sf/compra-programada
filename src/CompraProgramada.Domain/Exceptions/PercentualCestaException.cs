using CompraProgramada.Domain.Exceptions.Base;

namespace CompraProgramada.Domain.Exceptions;

public class PercentualCestaException : DomainException
{
    public PercentualCestaException(decimal percentualInformado)
        : base($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {percentualInformado}%.",
            "PERCENTUAIS_INVALIDOS")
    { }
}