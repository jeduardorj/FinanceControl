using AutoMapper;
using FinanceControl.Application.DTOs.User;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // CreateUserDto → User
        // Ignora PasswordHash pois será definido manualmente no Service
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        // User → UserResponseDto
        CreateMap<User, UserResponseDto>();
    }
}