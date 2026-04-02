using CompraProgramada.Shared.Exceptions.Base;

namespace CompraProgramada.Shared.Exceptions;

public class PercentualCestaException : DomainException
{
    public PercentualCestaException(decimal percentualInformado)
        : base($"A soma dos percentuais deve ser exatamente 100%. Soma atual: {percentualInformado}%.",
            "PERCENTUAIS_INVALIDOS")
    { }
}