namespace TodoApp.Application.Service
{
    /// <summary>
    /// Interface cho Email Service.
    /// Dùng để gửi email notifications trong các Event Handlers.
    /// </summary>
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Gửi email đến nhiều người
        /// </summary>
        Task<bool> SendEmailAsync(IEnumerable<string> toList, string subject, string body, bool isHtml = true);

        /// <summary>
        /// Gửi email chào mừng người dùng mới
        /// </summary>
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);

        /// <summary>
        /// Gửi email reset mật khẩu
        /// </summary>
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink);
    }

    public class EmailMessage
    {
        public string To { get; set; } = null!;
        public List<string> ToList { get; set; } = new();
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public bool IsHtml { get; set; } = true;
    }
}
