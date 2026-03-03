namespace GiaPha_Application.Common;

public interface IEmailSender
{
    Task SendEmail(string to, string subject, string body);
}