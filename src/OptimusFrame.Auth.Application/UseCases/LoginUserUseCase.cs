using OptimusFrame.Auth.Application.Interfaces;

namespace OptimusFrame.Auth.Application.UseCases
{
    public class LoginUserUseCase(IIdentityProvider identityProvider)
    {
        private readonly IIdentityProvider _identityProvider = identityProvider;

        public async Task<string> ExecuteAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            return await _identityProvider.LoginAsync(email, password);
        }
    }
}
