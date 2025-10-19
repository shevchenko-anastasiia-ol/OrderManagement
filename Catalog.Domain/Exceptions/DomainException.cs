namespace Catalog.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public string EntityId { get; }

    public NotFoundException(string entityName, string entityId) 
        : base($"Entity '{entityName}' with id '{entityId}' not found")
    {
        EntityName = entityName;
        EntityId = entityId;
    }
}