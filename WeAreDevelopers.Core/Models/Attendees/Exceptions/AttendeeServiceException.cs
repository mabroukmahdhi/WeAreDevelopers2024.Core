using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeServiceException : Xeption
    {
        public AttendeeServiceException(Exception innerException)
            : base(message: "Attendee service error occurred, contact support.", innerException)
        { }
    }
}