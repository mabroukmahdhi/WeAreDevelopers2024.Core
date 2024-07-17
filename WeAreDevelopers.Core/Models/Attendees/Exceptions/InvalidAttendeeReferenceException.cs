// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

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