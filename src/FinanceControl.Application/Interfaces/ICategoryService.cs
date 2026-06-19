using FinanceControl.Application.DTOs.Category;

namespace FinanceControl.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateAsync(Guid userId, CreateCategoryDto dto);
    Task<IEnumerable<CategoryResponseDto>> GetAllByUserIdAsync(Guid userId);
    Task<CategoryResponseDto> GetByIdAsync(Guid userId, Guid categoryId);
    Task<CategoryResponseDto> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryDto dto);
    Task DeleteAsync(Guid userId, Guid categoryId);
}