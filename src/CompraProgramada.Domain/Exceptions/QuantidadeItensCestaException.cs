using CompraProgramada.Domain.Exceptions.Base;

namespace CompraProgramada.Domain.Exceptions;

public class QuantidadeItensCestaException : DomainException
{
    public QuantidadeItensCestaException(int quantidadeExata)
        : base($"A cesta deve conter exatamente 5 ativos. Quantidade informada: {quantidadeExata}.",
            "QUANTIDADE_ATIVOS_INVALIDA")
    { }
}