using CompraProgramada.Application.Contract.Service;
using System.Diagnostics.CodeAnalysis;

namespace CompraProgramada.Application.Service;

[ExcludeFromCodeCoverage]
public class DateTimeProvaider : IDateTimeProvaider
{
    public DateTime Now => DateTime.Now;
}