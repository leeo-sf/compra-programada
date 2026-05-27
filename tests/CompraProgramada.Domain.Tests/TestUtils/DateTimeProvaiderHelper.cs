using CompraProgramada.Domain.Contract.Service;

namespace CompraProgramada.Domain.Tests.TestUtils;

public class DateTimeProvaiderHelper : IDateTimeProvaider
{
    public DateTime Now { get; }

    public DateTimeProvaiderHelper(DateTime date) => Now = date;
}