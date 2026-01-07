namespace Order_Service.src._01_Domain.Core.Events
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
