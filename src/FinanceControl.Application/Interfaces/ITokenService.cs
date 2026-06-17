using FinanceControl.Application.DTOs.Auth;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Interfaces;

public interface ITokenService
{
    TokenResponseDto GenerateToken(User user);
}