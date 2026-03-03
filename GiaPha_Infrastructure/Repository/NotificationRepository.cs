using GiaPha_Application.Repository;
using GiaPha_Domain.Entities;
using GiaPha_Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace GiaPha_Infrastructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DbGiaPha _context;
        public NotificationRepository(DbGiaPha context) => _context = context;

        public async Task AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
        }

        public async Task<IReadOnlyList<Notification>> GetAllForUserAsync(Guid userId, Guid? hoId)
        {
            return await _context.Notifications
                .Where(n => n.IsGlobal 
                         || (hoId.HasValue && n.HoId.HasValue && n.HoId == hoId.Value))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Notification>> GetAllForAdminAsync()
        {
            return await _context.Notifications
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                typeof(Notification).GetProperty("DaDoc")?.SetValue(notification, true);
            }
        }
    }
}