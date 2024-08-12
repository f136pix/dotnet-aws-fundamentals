using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerDeletedHandler : IRequestHandler<CustomerDeleted>
{
    public Task Handle(CustomerDeleted request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Customer deleted !!");
        return Task.CompletedTask;
    }
}