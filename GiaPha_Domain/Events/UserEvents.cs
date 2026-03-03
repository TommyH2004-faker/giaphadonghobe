

using GiaPha_Domain.Common;

namespace GiaPha_Domain.Events
{
    public static class UserEvents
    {
        public record UserRegistered : DomainEventBase
        {
            public Guid id{ get; init; }
            public string Email { get; init; }
            public string TenDangNhap { get; init; }
            public string ActivationCode { get; init; }
            
            public UserRegistered(Guid  idUser, string email, string tenDangNhap, string activationCode)
            {
                id = idUser;
                Email = email;
                TenDangNhap = tenDangNhap;
                ActivationCode = activationCode;
            }
        }

        public record UserActivated : DomainEventBase
        {
            public Guid id{ get; init; }
            public string Email { get; init; }
            public DateTime ActivatedAt { get; init; }
            
            public UserActivated(Guid idUser, string email, DateTime activatedAt)
            {
                id = idUser;
                Email = email;
                ActivatedAt = activatedAt;
            }
        }
        public record UserPasswordChanged : DomainEventBase
        {
            public Guid id{ get; init; }
            public string Email { get; init; }
            public DateTime ChangedAt { get; init; }
            
            public UserPasswordChanged(Guid idUser, string email, DateTime changedAt)
            {
                id = idUser;
                Email = email;
                ChangedAt = changedAt;
            }
        }
        
        public record UserForgotPassword : DomainEventBase
        {
             public Guid Id { get; }
            public string Email { get; }
            public string plainPassword { get; init; } = null!;
            public new DateTime OccurredOn { get; }
            public UserForgotPassword(Guid id, string email, string plainPassword, DateTime utcNow)
            {
                Id = id;
                Email = email;
                this.plainPassword = plainPassword;
                OccurredOn = DateTime.UtcNow;
            }
        }
    }
}