using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.Extensions.Configuration;
using OptimusFrame.Auth.Application.Interfaces;

namespace OptimusFrame.Auth.Infrastructure.Services
{
    public class CognitoIdentityProvider(
            IAmazonCognitoIdentityProvider provider,
            IConfiguration configuration) : IIdentityProvider
    {
        private readonly IAmazonCognitoIdentityProvider _provider = provider;
        private readonly string _clientId = configuration["AWS:Cognito:ClientId"] 
            ?? throw new ArgumentNullException(nameof(configuration), "AWS:Cognito:ClientId configuration is missing.");

        public async Task<string> RegisterAsync(string email, string password)
        {
            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Username = email,
                Password = password,
                UserAttributes =
                [
                    new AttributeType
                        {
                            Name = "email",
                            Value = email
                        }
                ]
            };

            var response = await _provider.SignUpAsync(request);

            return response.UserSub;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var request = new InitiateAuthRequest
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = _clientId,
                AuthParameters = new Dictionary<string, string>
                    {
                        { "USERNAME", email },
                        { "PASSWORD", password }
                    }
            };

            var response = await _provider.InitiateAuthAsync(request);

            return response.AuthenticationResult.IdToken;
        }
    }
}
