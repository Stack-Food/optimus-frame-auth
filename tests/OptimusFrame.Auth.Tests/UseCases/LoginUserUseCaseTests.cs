using FluentAssertions;
using Moq;
using OptimusFrame.Auth.Application.Interfaces;
using OptimusFrame.Auth.Application.UseCases;

namespace OptimusFrame.Auth.Tests.UseCases
{
    public class LoginUserUseCaseTests
    {
        private readonly Mock<IIdentityProvider> _identityProviderMock;
        private readonly LoginUserUseCase _useCase;

        public LoginUserUseCaseTests()
        {
            _identityProviderMock = new Mock<IIdentityProvider>();
            _useCase = new LoginUserUseCase(_identityProviderMock.Object);
        }

        [Fact]
        public async Task Should_Return_Token_When_Login_Is_Successful()
        {
            // Arrange
            var email = "test@email.com";
            var password = "123456";
            var expectedToken = "jwt_token";

            _identityProviderMock
                .Setup(x => x.LoginAsync(email, password))
                .ReturnsAsync(expectedToken);

            // Act
            var result = await _useCase.ExecuteAsync(email, password);

            // Assert
            result.Should().Be(expectedToken);
            _identityProviderMock.Verify(
                x => x.LoginAsync(email, password),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Email_Is_Empty()
        {
            // Act
            Func<Task> act = async () =>
                await _useCase.ExecuteAsync("", "123");

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Email is required");
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Password_Is_Empty()
        {
            Func<Task> act = async () =>
                await _useCase.ExecuteAsync("test@email.com", "");

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Password is required");
        }
    }

}
