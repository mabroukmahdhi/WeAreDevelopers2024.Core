// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

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