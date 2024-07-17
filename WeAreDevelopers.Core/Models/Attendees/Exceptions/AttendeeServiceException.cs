// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

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