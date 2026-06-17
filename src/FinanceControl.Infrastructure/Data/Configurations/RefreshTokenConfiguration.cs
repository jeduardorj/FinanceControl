
using FinanceControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceControl.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(r => r.Token)
            .IsUnique();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Ignora propriedades calculadas
        builder.Ignore(r => r.IsExpired);
        builder.Ignore(r => r.IsActive);

        // Relacionamento: RefreshToken pertence a User
        builder.HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Soft delete não faz sentido para tokens
        // mas vou manter o IsDeleted da BaseEntity
    }
}