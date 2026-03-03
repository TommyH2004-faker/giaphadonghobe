using GiaPha_Domain.Entities;

namespace GiaPha_Application.Repository
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IReadOnlyList<Notification>> GetAllForUserAsync(Guid userId, Guid? hoId);
        Task<IReadOnlyList<Notification>> GetAllForAdminAsync();
        Task MarkAsReadAsync(Guid notificationId);
    }
}