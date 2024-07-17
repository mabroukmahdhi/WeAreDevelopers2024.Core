using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AlreadyExistsAttendeeException : Xeption
    {
        public AlreadyExistsAttendeeException(Exception innerException)
            : base(message: "Attendee with the same Id already exists.", innerException)
        { }
    }
}