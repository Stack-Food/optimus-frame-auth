using FluentAssertions;
using Moq;
using OptimusFrame.Auth.Application.Interfaces;
using OptimusFrame.Auth.Application.UseCases;

namespace OptimusFrame.Auth.Tests.UseCases
{
    public class RegisterUserUseCaseTests
    {
        private readonly Mock<IIdentityProvider> _identityProviderMock;
        private readonly RegisterUserUseCase _useCase;

        public RegisterUserUseCaseTests()
        {
            _identityProviderMock = new Mock<IIdentityProvider>();
            _useCase = new RegisterUserUseCase(_identityProviderMock.Object);
        }

        [Fact]
        public async Task Should_Register_User_When_Data_Is_Valid()
        {
            // Arrange
            var email = "test@email.com";
            var password = "123456";
            var expectedUserId = "user-sub-id";

            _identityProviderMock
                .Setup(x => x.RegisterAsync(email, password))
                .ReturnsAsync(expectedUserId);

            // Act
            var result = await _useCase.ExecuteAsync(email, password);

            // Assert
            result.Should().Be(expectedUserId);
            _identityProviderMock.Verify(
                x => x.RegisterAsync(email, password),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_When_Email_Is_Invalid()
        {
            Func<Task> act = async () =>
                await _useCase.ExecuteAsync("invalidemail", "123456");

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Invalid email format");
        }

        [Fact]
        public async Task Should_Throw_When_Password_Is_Too_Short()
        {
            Func<Task> act = async () =>
                await _useCase.ExecuteAsync("test@email.com", "123");

            await act.Should()
                .ThrowAsync<ArgumentException>()
                .WithMessage("Password must be at least 6 characters");
        }
    }

}
