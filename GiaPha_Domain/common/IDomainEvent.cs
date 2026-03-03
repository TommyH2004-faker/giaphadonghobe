namespace GiaPha_Domain.Common
{
    /// <summary>
   /// Interface đánh dấu một event trong hệ thống
   /// Là 1 hành vi ghi lại sự thay đổi trạng thái quan trọng trong Event 
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
