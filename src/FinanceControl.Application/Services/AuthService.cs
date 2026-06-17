using FinanceControl.Application.DTOs.Auth;
using FinanceControl.Application.Interfaces;
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
        // Busca o usuário pelo e-mail
        var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);

        if (user is null)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Verifica a senha com BCrypt
        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        // Gera e retorna o token
        return _tokenService.GenerateToken(user);
    }
}