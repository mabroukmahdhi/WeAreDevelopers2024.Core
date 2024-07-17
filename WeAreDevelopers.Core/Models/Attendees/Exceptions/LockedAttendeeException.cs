using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class LockedAttendeeException : Xeption
    {
        public LockedAttendeeException(Exception innerException)
            : base(message: "Locked Attendee record exception, please try again later", innerException)
        {
        }
    }
}