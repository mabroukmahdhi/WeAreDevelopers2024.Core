// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeDependencyException : Xeption
    {
        public AttendeeDependencyException(Xeption innerException) :
            base(message: "Attendee dependency error occurred, contact support.", innerException)
        { }
    }
}