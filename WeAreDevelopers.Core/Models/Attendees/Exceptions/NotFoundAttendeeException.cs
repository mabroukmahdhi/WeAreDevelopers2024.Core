using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class NotFoundAttendeeException : Xeption
    {
        public NotFoundAttendeeException(Guid AttendeeId)
            : base(message: $"Couldn't find Attendee with AttendeeId: {AttendeeId}.")
        { }
    }
}