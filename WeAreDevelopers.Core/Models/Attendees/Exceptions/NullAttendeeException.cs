// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class NullAttendeeException : Xeption
    {
        public NullAttendeeException()
            : base(message: "Attendee is null.")
        { }
    }
}