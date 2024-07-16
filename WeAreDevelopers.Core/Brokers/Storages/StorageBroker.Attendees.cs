// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Attendee> Attendees { get; set; }

        public async ValueTask<Attendee> InsertAttendeeAsync(Attendee Attendee)
        {
            EntityEntry<Attendee> AttendeeEntityEntry =
                await Attendees.AddAsync(Attendee);

            await SaveChangesAsync();

            return AttendeeEntityEntry.Entity;
        }

        public IQueryable<Attendee> SelectAllAttendees() => this.Attendees;

        public async ValueTask<Attendee> SelectAttendeeByIdAsync(Guid AttendeeId) =>
            await Attendees.FindAsync(AttendeeId);

        public async ValueTask<Attendee> UpdateAttendeeAsync(Attendee Attendee)
        {
            EntityEntry<Attendee> AttendeeEntityEntry =
                Attendees.Update(Attendee);

            await SaveChangesAsync();

            return AttendeeEntityEntry.Entity;
        }

        public async ValueTask<Attendee> DeleteAttendeeAsync(Attendee Attendee)
        {
            EntityEntry<Attendee> AttendeeEntityEntry =
                Attendees.Remove(Attendee);

            await SaveChangesAsync();

            return AttendeeEntityEntry.Entity;
        }
    }
}
