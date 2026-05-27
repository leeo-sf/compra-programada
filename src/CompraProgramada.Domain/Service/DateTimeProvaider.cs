using CompraProgramada.Domain.Contract.Service;
using System.Diagnostics.CodeAnalysis;

namespace CompraProgramada.Domain.Service;

[ExcludeFromCodeCoverage]
public class DateTimeProvaider : IDateTimeProvaider
{
    public DateTime Now => DateTime.Now;
}