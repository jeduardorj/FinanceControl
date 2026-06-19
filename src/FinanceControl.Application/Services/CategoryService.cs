using AutoMapper;
using FinanceControl.Application.DTOs.Category;
using FinanceControl.Application.Interfaces;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Exceptions;
using FinanceControl.Domain.Interfaces;

namespace FinanceControl.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryResponseDto> CreateAsync(Guid userId, CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        category.UserId = userId;

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllByUserIdAsync(Guid userId)
    {
        var categories = await _unitOfWork.Categories.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> GetByIdAsync(Guid userId, Guid categoryId)
    {
        var category = await GetCategoryAndValidateOwnership(userId, categoryId);
        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(
        Guid userId, Guid categoryId, UpdateCategoryDto dto)
    {
        var category = await GetCategoryAndValidateOwnership(userId, categoryId);

        category.Name = dto.Name;
        category.Color = dto.Color;
        category.Icon = dto.Icon;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteAsync(Guid userId, Guid categoryId)
    {
        var category = await GetCategoryAndValidateOwnership(userId, categoryId);

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.CommitAsync();
    }

    // Método privado reutilizado por GetById, Update e Delete
    private async Task<Category> GetCategoryAndValidateOwnership(
        Guid userId, Guid categoryId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

        if (category is null || category.UserId != userId)
            throw new NotFoundException("Categoria", categoryId);

        return category;
    }
}