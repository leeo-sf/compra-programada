using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Tests.TestUtils;

public class DateTimeProvaiderFaker : IDateTimeProvaider
{
    public DateTime Now { get; }

    public DateTimeProvaiderFaker(DateTime date) => Now = date;
}