// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Attendee> InsertAttendeeAsync(Attendee Attendee);
        IQueryable<Attendee> SelectAllAttendees();
        ValueTask<Attendee> SelectAttendeeByIdAsync(Guid AttendeeId);
        ValueTask<Attendee> UpdateAttendeeAsync(Attendee Attendee);
        ValueTask<Attendee> DeleteAttendeeAsync(Attendee Attendee);
    }
}
