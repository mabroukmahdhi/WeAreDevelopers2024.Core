using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeDependencyException : Xeption
    {
        public AttendeeDependencyException(Xeption innerException) :
            base(message: "Attendee dependency error occurred, contact support.", innerException)
        { }
    }
}