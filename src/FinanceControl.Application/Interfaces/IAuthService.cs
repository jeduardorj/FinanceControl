using FinanceControl.Application.DTOs.Auth;

namespace FinanceControl.Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
}