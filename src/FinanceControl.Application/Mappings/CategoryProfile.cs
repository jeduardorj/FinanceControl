using AutoMapper;
using FinanceControl.Application.DTOs.Category;
using FinanceControl.Domain.Entities;

namespace FinanceControl.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        CreateMap<Category, CategoryResponseDto>();
    }
}