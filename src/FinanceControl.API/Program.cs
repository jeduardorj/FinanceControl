using FinanceControl.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using FinanceControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

using FinanceControl.Application.Interfaces;
using FinanceControl.Application.Services;
using FluentValidation;
//using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// AutoMapper — escaneia o assembly do Application em busca de Profiles
builder.Services.AddAutoMapper(cfg => { }, typeof(FinanceControl.Application.Mappings.UserProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<FinanceControl.Application.Validators.CreateUserDtoValidator>();

// Application Services
builder.Services.AddScoped<IUserService, UserService>();

// Repository Pattern e Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();