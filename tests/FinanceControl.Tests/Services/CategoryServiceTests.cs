using AutoMapper;
using FinanceControl.Application.DTOs.Category;
using FinanceControl.Application.Services;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Exceptions;
using FinanceControl.Domain.Interfaces;
using FinanceControl.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace FinanceControl.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _categoryService;
    private readonly Guid _userId = Guid.NewGuid();

    public CategoryServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(x => x.Categories)
            .Returns(_mockCategoryRepository.Object);

        _categoryService = new CategoryService(
            _mockUnitOfWork.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldCallCommitAsync()
    {
        var dto = new CreateCategoryDto
        {
            Name = "Alimentação",
            Color = "#FF5733",
            Icon = "restaurant"
        };

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Color = dto.Color,
            Icon = dto.Icon,
            UserId = _userId
        };

        _mockMapper.Setup(x => x.Map<Category>(dto)).Returns(category);
        _mockMapper.Setup(x => x.Map<CategoryResponseDto>(It.IsAny<Category>()))
            .Returns(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color
            });

        _mockCategoryRepository
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);

        await _categoryService.CreateAsync(_userId, dto);

        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryBelongsToUser_ShouldReturnCategory()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            UserId = _userId,
            Name = "Alimentação",
            Color = "#FF5733",
            Icon = "restaurant"
        };

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockMapper.Setup(x => x.Map<CategoryResponseDto>(category))
            .Returns(new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            });

        var result = await _categoryService.GetByIdAsync(_userId, categoryId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Alimentação");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotBelongToUser_ShouldThrowNotFoundException()
    {
        var categoryId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var category = new Category
        {
            Id = categoryId,
            UserId = otherUserId,
            Name = "Alimentação"
        };

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        var action = async () =>
            await _categoryService.GetByIdAsync(_userId, categoryId);

        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ShouldSoftDelete()
    {
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            UserId = _userId,
            Name = "Alimentação"
        };

        _mockCategoryRepository
            .Setup(x => x.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mockCategoryRepository
            .Setup(x => x.Delete(It.IsAny<Category>()));

        _mockUnitOfWork.Setup(x => x.CommitAsync()).ReturnsAsync(1);

        await _categoryService.DeleteAsync(_userId, categoryId);

        _mockCategoryRepository.Verify(
            x => x.Delete(It.IsAny<Category>()), Times.Once);
        _mockUnitOfWork.Verify(
            x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_ShouldReturnOnlyUserCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), UserId = _userId, Name = "Alimentação" },
            new() { Id = Guid.NewGuid(), UserId = _userId, Name = "Transporte" }
        };

        _mockCategoryRepository
            .Setup(x => x.GetByUserIdAsync(_userId))
            .ReturnsAsync(categories);

        _mockMapper
            .Setup(x => x.Map<IEnumerable<CategoryResponseDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(categories.Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            }));

        var result = await _categoryService.GetAllByUserIdAsync(_userId);

        result.Should().HaveCount(2);
    }
}