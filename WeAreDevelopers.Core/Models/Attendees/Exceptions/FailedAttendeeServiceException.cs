using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class FailedAttendeeServiceException : Xeption
    {
        public FailedAttendeeServiceException(Exception innerException)
            : base(message: "Failed Attendee service occurred, please contact support", innerException)
        { }
    }
}