// کد صحیح
using Identity_Service.Domain.Enums;
using MediatR;

namespace Identity_Service.Application.Common.Events
{
    public class UserRegisteredEvent : INotification // <-- این خط را اضافه کنید
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }

        // Constructor را هم اضافه کنید تا بهتر باشد
        public UserRegisteredEvent(Guid id, string username, string email, string firstName, string lastName, UserStatus status)
        {
            Id = id;
            Username = username;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Status = status;
        }
    }
}