

namespace FinanceControl.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    //Chave estrangeira para o usuário
    public Guid UserId { get; set; }

    //Propriedade de navegação para o usuário
    public User User { get; set; } = null!;

    //Propriedade calculada - não persiste no banco de dados
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
