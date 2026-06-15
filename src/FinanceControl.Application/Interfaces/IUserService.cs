using FinanceControl.Application.DTOs.User;

namespace FinanceControl.Application.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(CreateUserDto dto);
}