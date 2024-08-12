using Customers.Consumer.Messages;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
{
    public Task Handle(CustomerCreated request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Customer created !!");
        return Task.CompletedTask;
    }
}