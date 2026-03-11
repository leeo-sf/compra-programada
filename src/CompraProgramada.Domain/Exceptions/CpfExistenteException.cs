using CompraProgramada.Domain.Exceptions.Base;

namespace CompraProgramada.Domain.Exceptions;

public class CpfExistenteException : DomainException
{
    public CpfExistenteException()
        : base("CPF ja cadastrado no sistema.",
            "CLIENTE_CPF_DUPLICADO")
    { }
}