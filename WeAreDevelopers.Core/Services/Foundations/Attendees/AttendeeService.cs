// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using WeAreDevelopers.Core.Brokers.DateTimes;
using WeAreDevelopers.Core.Brokers.Loggings;
using WeAreDevelopers.Core.Brokers.Storages;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Services.Foundations.Attendees
{
    public partial class AttendeeService : IAttendeeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public AttendeeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Attendee> AddAttendeeAsync(Attendee Attendee) =>
            TryCatch(async () =>
            {
                ValidateAttendeeOnAdd(Attendee);

                return await this.storageBroker.InsertAttendeeAsync(Attendee);
            });

        public IQueryable<Attendee> RetrieveAllAttendees() =>
            TryCatch(() => this.storageBroker.SelectAllAttendees());

        public ValueTask<Attendee> RetrieveAttendeeByIdAsync(Guid AttendeeId) =>
            TryCatch(async () =>
            {
                ValidateAttendeeId(AttendeeId);

                Attendee maybeAttendee = await this.storageBroker
                    .SelectAttendeeByIdAsync(AttendeeId);

                ValidateStorageAttendee(maybeAttendee, AttendeeId);

                return maybeAttendee;
            });

        public ValueTask<Attendee> ModifyAttendeeAsync(Attendee Attendee) =>
            TryCatch(async () =>
            {
                ValidateAttendeeOnModify(Attendee);

                Attendee maybeAttendee =
                    await this.storageBroker.SelectAttendeeByIdAsync(Attendee.Id);

                ValidateStorageAttendee(maybeAttendee, Attendee.Id);
                ValidateAgainstStorageAttendeeOnModify(inputAttendee: Attendee, storageAttendee: maybeAttendee);

                return await this.storageBroker.UpdateAttendeeAsync(Attendee);
            });

        public ValueTask<Attendee> RemoveAttendeeByIdAsync(Guid AttendeeId) =>
            TryCatch(async () =>
            {
                ValidateAttendeeId(AttendeeId);

                Attendee maybeAttendee = await this.storageBroker
                    .SelectAttendeeByIdAsync(AttendeeId);

                ValidateStorageAttendee(maybeAttendee, AttendeeId);

                return await this.storageBroker.DeleteAttendeeAsync(maybeAttendee);
            });
    }
}