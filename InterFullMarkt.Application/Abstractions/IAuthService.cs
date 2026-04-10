namespace InterFullMarkt.Application.Abstractions;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}