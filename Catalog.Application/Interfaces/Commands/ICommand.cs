using MediatR;

namespace Catalog.Application.Interfaces.Commands;

public interface ICommand : IRequest<Unit> { }
public interface ICommand<out TResponse> : IRequest<TResponse> { }