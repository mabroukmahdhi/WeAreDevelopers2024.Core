// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Services.Foundations.Attendees
{
    public interface IAttendeeService
    {
        ValueTask<Attendee> AddAttendeeAsync(Attendee Attendee);
        IQueryable<Attendee> RetrieveAllAttendees();
        ValueTask<Attendee> RetrieveAttendeeByIdAsync(Guid AttendeeId);
        ValueTask<Attendee> ModifyAttendeeAsync(Attendee Attendee);
        ValueTask<Attendee> RemoveAttendeeByIdAsync(Guid AttendeeId);
    }
}