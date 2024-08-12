using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerUpdatedHandler : IRequestHandler<CustomerUpdated>
{
    public Task Handle(CustomerUpdated request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Customer updated !!");
        return Task.CompletedTask;
    }
}