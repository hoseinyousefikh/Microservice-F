using SharedKernel.Domain.Primitives;

namespace Identity_Service.Application.Common.Events
{
    public class UserPasswordChangedEvent : DomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }

        public UserPasswordChangedEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}