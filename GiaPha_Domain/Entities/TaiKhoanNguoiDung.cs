using GiaPha_Domain.Common;
using static GiaPha_Domain.Events.UserEvents;

namespace GiaPha_Domain.Entities;

public class TaiKhoanNguoiDung :IHasDomainEvents
{
    public Guid Id { get; private set; }
    public string TenDangNhap { get; private set; } = null!;
    public string MatKhau { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Avatar { get; private set; }
    public bool GioiTinh { get; private set; }
    public string? SoDienThoai { get; private set; }
    public string Role { get; private set; } = "User";
    public bool Enabled { get; private set; }
    public string? ActivationCode { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }

    // Navigation - Many to Many với Ho qua TaiKhoan_Ho
    public ICollection<TaiKhoan_Ho> TaiKhoan_Hos { get; set; } = new List<TaiKhoan_Ho>();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => DomainEventsInternal.AsReadOnly();
    private List<IDomainEvent> DomainEventsInternal { get; } = new List<IDomainEvent>();

    private TaiKhoanNguoiDung() { }

     public static TaiKhoanNguoiDung Register(
        string TenDangNhap,
        string email,
        bool gioiTinh,
        string MatKhauMaHoa,
        string? SoDienThoai,
        string role)
        {
            if (string.IsNullOrWhiteSpace(TenDangNhap))
                throw new ArgumentException("TenDangNhap is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(MatKhauMaHoa))
                throw new ArgumentException("MatKhauMaHoa is required");
            // Tạo activation code 6 số
            var activationCode = new Random().Next(100000, 999999).ToString();

            var user = new TaiKhoanNguoiDung
            {
                Id = Guid.NewGuid(),
                TenDangNhap = TenDangNhap,
                Email = email,
                MatKhau = MatKhauMaHoa,
                Role = role,
                GioiTinh = gioiTinh,
                Enabled = false,
                ActivationCode = activationCode
            };
            
            
            user.AddDomainEvent(new UserRegistered(
                user.Id,
                user.Email,
                user.TenDangNhap,
                user.ActivationCode
            ));
            
            return user;
        }
        public void UpdateAvatar(string avatar)
        {
            Avatar = avatar;
        }
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("PasswordHash required");

            MatKhau = newPasswordHash;
            
            // ⚡ Raise Domain Event
            AddDomainEvent(new UserPasswordChanged(
                this.Id,
                this.Email,
                DateTime.UtcNow
            ));
        }

        public void Activate()
        {
            if (Enabled)
                throw new InvalidOperationException("User is already activated");
            
            Enabled = true;
            ActivationCode = null;
            AddDomainEvent(new UserActivated(
                this.Id,
                this.Email,
                DateTime.UtcNow
            ));
        }

        public void ChangeRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role required");

            Role = role;
        }

        public void SetRefreshToken(string refreshToken, DateTime refreshTokenExpiry)
        {
    
            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiry = refreshTokenExpiry;
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            DomainEventsInternal.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            DomainEventsInternal.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            DomainEventsInternal.Clear();
        }

        public void ForgotPassword(string newPasswordHash, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("New password hash required");

            MatKhau = newPasswordHash;
            
            // ⚡ Raise Domain Event
            AddDomainEvent(new UserForgotPassword(
                this.Id,
                this.Email,
                plainPassword,
                DateTime.UtcNow
            ));
        }
}
