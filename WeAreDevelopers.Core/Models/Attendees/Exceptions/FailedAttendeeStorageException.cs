// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class FailedAttendeeStorageException : Xeption
    {
        public FailedAttendeeStorageException(Exception innerException)
            : base(message: "Failed Attendee storage error occurred, contact support.", innerException)
        { }
    }
}