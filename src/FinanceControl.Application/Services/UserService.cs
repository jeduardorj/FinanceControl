using AutoMapper;
using FinanceControl.Application.DTOs.User;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Exceptions;
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
        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(dto.Email);

        if (emailExists)
            throw new ConflictException("Este e-mail já está em uso.");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<UserResponseDto>(user);
    }
}