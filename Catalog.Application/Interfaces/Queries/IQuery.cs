using MediatR;

namespace Catalog.Application.Interfaces.Queries;

public interface IQuery<out TResponse> : IRequest<TResponse> { }