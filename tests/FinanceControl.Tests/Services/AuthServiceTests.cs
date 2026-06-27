using FinanceControl.Application.DTOs.Auth;
using FinanceControl.Application.Interfaces;
using FinanceControl.Application.Services;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Exceptions;
using FinanceControl.Domain.Interfaces;
using FinanceControl.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace FinanceControl.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepository;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTokenService = new Mock<ITokenService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();

        _mockUnitOfWork.Setup(x => x.Users)
            .Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(x => x.RefreshTokens)
            .Returns(_mockRefreshTokenRepository.Object);

        _authService = new AuthService(
            _mockUnitOfWork.Object,
            _mockTokenService.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnTokenResponse()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "joao@email.com",
            Password = "senha123"
        };

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("senha123");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "João Silva",
            Email = "joao@email.com",
            PasswordHash = passwordHash
        };

        var expectedResponse = new TokenResponseDto
        {
            AccessToken = "fake-token",
            RefreshToken = "fake-refresh-token",
            Name = user.Name,
            Email = user.Email
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        _mockTokenService
            .Setup(x => x.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        _mockRefreshTokenRepository
            .Setup(x => x.AddAsync(It.IsAny<RefreshToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.CommitAsync())
            .ReturnsAsync(1);

        _mockTokenService
            .Setup(x => x.GenerateToken(user, It.IsAny<string>()))
            .Returns(expectedResponse);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("fake-token");
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "naoexiste@email.com",
            Password = "senha123"
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var action = async () => await _authService.LoginAsync(dto);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("*inválidas*");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var dto = new LoginDto
        {
            Email = "joao@email.com",
            Password = "senhaerrada"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "joao@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123")
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        // Act
        var action = async () => await _authService.LoginAsync(dto);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("*inválidas*");
    }
}