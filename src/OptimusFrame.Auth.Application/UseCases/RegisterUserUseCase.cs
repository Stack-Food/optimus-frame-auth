using OptimusFrame.Auth.Application.Interfaces;

namespace OptimusFrame.Auth.Application.UseCases
{
    public class RegisterUserUseCase(IIdentityProvider identityProvider)
    {
        private readonly IIdentityProvider _identityProvider = identityProvider;

        public async Task<string> ExecuteAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            return await _identityProvider.RegisterAsync(email, password);
        }
    }

}
