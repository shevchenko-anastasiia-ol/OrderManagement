namespace WarehouseBLL.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}