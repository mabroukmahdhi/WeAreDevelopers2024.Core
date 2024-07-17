using System;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Services.Foundations.Attendees
{
    public partial class AttendeeService
    {
        private void ValidateAttendeeOnAdd(Attendee Attendee)
        {
            ValidateAttendeeIsNotNull(Attendee);

            Validate(
                (Rule: IsInvalid(Attendee.Id), Parameter: nameof(Attendee.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(Attendee.CreatedDate), Parameter: nameof(Attendee.CreatedDate)),
                (Rule: IsInvalid(Attendee.CreatedByUserId), Parameter: nameof(Attendee.CreatedByUserId)),
                (Rule: IsInvalid(Attendee.UpdatedDate), Parameter: nameof(Attendee.UpdatedDate)),
                (Rule: IsInvalid(Attendee.UpdatedByUserId), Parameter: nameof(Attendee.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: Attendee.UpdatedDate,
                    secondDate: Attendee.CreatedDate,
                    secondDateName: nameof(Attendee.CreatedDate)),
                Parameter: nameof(Attendee.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: Attendee.UpdatedByUserId,
                    secondId: Attendee.CreatedByUserId,
                    secondIdName: nameof(Attendee.CreatedByUserId)),
                Parameter: nameof(Attendee.UpdatedByUserId)),

                (Rule: IsNotRecent(Attendee.CreatedDate), Parameter: nameof(Attendee.CreatedDate)));
        }

        private void ValidateAttendeeOnModify(Attendee Attendee)
        {
            ValidateAttendeeIsNotNull(Attendee);

            Validate(
                (Rule: IsInvalid(Attendee.Id), Parameter: nameof(Attendee.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(Attendee.CreatedDate), Parameter: nameof(Attendee.CreatedDate)),
                (Rule: IsInvalid(Attendee.CreatedByUserId), Parameter: nameof(Attendee.CreatedByUserId)),
                (Rule: IsInvalid(Attendee.UpdatedDate), Parameter: nameof(Attendee.UpdatedDate)),
                (Rule: IsInvalid(Attendee.UpdatedByUserId), Parameter: nameof(Attendee.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: Attendee.UpdatedDate,
                    secondDate: Attendee.CreatedDate,
                    secondDateName: nameof(Attendee.CreatedDate)),
                Parameter: nameof(Attendee.UpdatedDate)),

                (Rule: IsNotRecent(Attendee.UpdatedDate), Parameter: nameof(Attendee.UpdatedDate)));
        }

        public void ValidateAttendeeId(Guid AttendeeId) =>
            Validate((Rule: IsInvalid(AttendeeId), Parameter: nameof(Attendee.Id)));

        private static void ValidateStorageAttendee(Attendee maybeAttendee, Guid AttendeeId)
        {
            if (maybeAttendee is null)
            {
                throw new NotFoundAttendeeException(AttendeeId);
            }
        }

        private static void ValidateAttendeeIsNotNull(Attendee Attendee)
        {
            if (Attendee is null)
            {
                throw new NullAttendeeException();
            }
        }

        private static void ValidateAgainstStorageAttendeeOnModify(Attendee inputAttendee, Attendee storageAttendee)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputAttendee.CreatedDate,
                    secondDate: storageAttendee.CreatedDate,
                    secondDateName: nameof(Attendee.CreatedDate)),
                Parameter: nameof(Attendee.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputAttendee.CreatedByUserId,
                    secondId: storageAttendee.CreatedByUserId,
                    secondIdName: nameof(Attendee.CreatedByUserId)),
                Parameter: nameof(Attendee.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputAttendee.UpdatedDate,
                    secondDate: storageAttendee.UpdatedDate,
                    secondDateName: nameof(Attendee.UpdatedDate)),
                Parameter: nameof(Attendee.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidAttendeeException = new InvalidAttendeeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidAttendeeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidAttendeeException.ThrowIfContainsErrors();
        }
    }
}