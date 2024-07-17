using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class InvalidAttendeeReferenceException : Xeption
    {
        public InvalidAttendeeReferenceException(Exception innerException)
            : base(message: "Invalid Attendee reference error occurred.", innerException) { }
    }
}