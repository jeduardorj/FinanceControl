using FinanceControl.Application.DTOs.Auth;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces;

namespace FinanceControl.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        // Gera o Refresh Token
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
        await _unitOfWork.CommitAsync();

        return _tokenService.GenerateToken(user, refreshTokenValue);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _unitOfWork.RefreshTokens
            .GetByTokenAsync(refreshToken);

        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("Refresh Token inválido ou expirado.");

        var user = token.User;

        // Revoga o token atual (rotação)
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(token);

        // Gera novo Refresh Token
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
        await _unitOfWork.CommitAsync();

        return _tokenService.GenerateToken(user, newRefreshTokenValue);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _unitOfWork.RefreshTokens
            .GetByTokenAsync(refreshToken);

        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("Refresh Token inválido.");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(token);
        await _unitOfWork.CommitAsync();
    }
}