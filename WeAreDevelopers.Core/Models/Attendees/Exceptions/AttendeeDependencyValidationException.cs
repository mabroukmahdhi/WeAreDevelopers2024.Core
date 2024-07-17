// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using Xeptions;

namespace WeAreDevelopers.Core.Models.Attendees.Exceptions
{
    public class AttendeeDependencyValidationException : Xeption
    {
        public AttendeeDependencyValidationException(Xeption innerException)
            : base(message: "Attendee dependency validation occurred, please try again.", innerException)
        { }
    }
}