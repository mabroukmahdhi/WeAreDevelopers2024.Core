using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class NullAttendeeException : Xeption
    {
        public NullAttendeeException()
            : base(message: "Attendee is null.")
        { }
    }
}