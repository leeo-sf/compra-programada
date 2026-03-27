using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Tests.TestUtils;

public class DateTimeProvaiderHelper : IDateTimeProvaider
{
    public DateTime Now { get; }

    public DateTimeProvaiderHelper(DateTime date) => Now = date;
}