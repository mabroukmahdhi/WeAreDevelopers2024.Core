using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeDependencyValidationException : Xeption
    {
        public AttendeeDependencyValidationException(Xeption innerException)
            : base(message: "Attendee dependency validation occurred, please try again.", innerException)
        { }
    }
}