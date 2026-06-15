using AutoMapper;
using FinanceControl.Application.DTOs.User;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Interfaces;

namespace FinanceControl.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserResponseDto> RegisterAsync(CreateUserDto dto)
    {
        // Regra de negócio: e-mail não pode estar duplicado
        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(dto.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Este e-mail já está em uso.");
        }

        // Mapeia DTO para entidade
        var user = _mapper.Map<User>(dto);

        // Hash da senha — nunca salvamos em texto puro
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Persiste
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CommitAsync();

        // Retorna DTO de resposta
        return _mapper.Map<UserResponseDto>(user);
    }
}