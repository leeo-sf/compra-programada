using CompraProgramada.Application.Interface;

namespace CompraProgramada.Application.Service;

public class DateTimeProvaider : IDateTimeProvaider
{
    public DateTime Now => DateTime.Now;
}