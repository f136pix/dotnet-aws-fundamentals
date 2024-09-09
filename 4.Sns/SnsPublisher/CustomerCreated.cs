using Amazon.Runtime;

namespace SnsPublisher;

public class CustomerCreated
{
    public Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string Email { get; set; }

    public required DateTime DateOfBirth { get; set; }

    public required string GitHubUsername { get; set; }
}