namespace Cona40LiveChat;

public class AuthSettings
{
    public required string PasswordHash { get; init; } = string.Empty;

    public required string PasswordSalt { get; init; } = string.Empty;
}