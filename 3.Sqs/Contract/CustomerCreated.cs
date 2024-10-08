namespace Contract;

public class CustomerCreated
{
    public required Guid Id { get; init; }
    
    public required string FullName { get; init; }
    
    public required string Email { get; init; }
    
    public required string GithubUsername { get; init; }

    public DateTime DateOfBirth { get; init; }
}