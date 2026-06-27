using AutoMapper;
using FinanceControl.Application.DTOs.User;
using FinanceControl.Application.Mappings;
using FinanceControl.Application.Services;
using FinanceControl.Domain.Entities;
using FinanceControl.Domain.Exceptions;
using FinanceControl.Domain.Interfaces;
using FinanceControl.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace FinanceControl.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(x => x.Users).Returns(_mockUserRepository.Object);

        _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "senha123"
        };

        _mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(dto.Email))
            .ReturnsAsync(true);

        var action = async () => await _userService.RegisterAsync(dto);

        await action.Should().ThrowAsync<ConflictException>()
            .WithMessage("*e-mail*");
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailDoesNotExist_ShouldCallCommitAsync()
    {
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "senha123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email
        };

        _mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(dto.Email))
            .ReturnsAsync(false);

        _mockMapper
            .Setup(x => x.Map<User>(dto))
            .Returns(user);

        _mockMapper
            .Setup(x => x.Map<UserResponseDto>(It.IsAny<User>()))
            .Returns(new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.CommitAsync())
            .ReturnsAsync(1);

        await _userService.RegisterAsync(dto);

        _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailDoesNotExist_ShouldReturnUserResponseDto()
    {
        var dto = new CreateUserDto
        {
            Name = "João Silva",
            Email = "joao@email.com",
            Password = "senha123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email
        };

        var expectedResponse = new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        _mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(dto.Email))
            .ReturnsAsync(false);

        _mockMapper
            .Setup(x => x.Map<User>(dto))
            .Returns(user);

        _mockMapper
            .Setup(x => x.Map<UserResponseDto>(It.IsAny<User>()))
            .Returns(expectedResponse);

        _mockUserRepository
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.CommitAsync())
            .ReturnsAsync(1);

        var result = await _userService.RegisterAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Email.Should().Be(dto.Email);
    }
}