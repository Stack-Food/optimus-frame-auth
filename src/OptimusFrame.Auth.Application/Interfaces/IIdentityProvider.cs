namespace OptimusFrame.Auth.Application.Interfaces
{
    public interface IIdentityProvider
    {
        Task<string> RegisterAsync(string email, string password);
        Task<string> LoginAsync(string email, string password);
    }

}
