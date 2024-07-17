// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class InvalidAttendeeException : Xeption
    {
        public InvalidAttendeeException()
            : base(message: "Invalid Attendee. Please correct the errors and try again.")
        { }
    }
}