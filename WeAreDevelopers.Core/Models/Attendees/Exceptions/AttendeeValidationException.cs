using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeValidationException : Xeption
    {
        public AttendeeValidationException(Xeption innerException)
            : base(message: "Attendee validation errors occurred, please try again.",
                  innerException)
        { }
    }
}