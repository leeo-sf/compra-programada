using CompraProgramada.Application.Contract.Service;

namespace CompraProgramada.Application.Tests.TestUtils;

public class DateTimeProvaiderHelper : IDateTimeProvaider
{
    public DateTime Now { get; }

    public DateTimeProvaiderHelper(DateTime date) => Now = date;
}