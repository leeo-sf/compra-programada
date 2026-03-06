using CompraProgramada.Application.Dto;
using OperationResult;

namespace CompraProgramada.Application.Interface;

public interface IPrecoMedioService
{
    Result<decimal> CalcularPrecoMedio(PrecoMedioDto custodia);
}