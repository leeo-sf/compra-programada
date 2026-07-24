using CompraProgramada.Shared.Exceptions.Base;

namespace CompraProgramada.Shared.Exceptions;

public class CpfExistenteException : DomainException
{
    public CpfExistenteException()
        : base("CPF já cadastrado no sistema",
            "CLIENTE_CPF_DUPLICADO")
    { }
}