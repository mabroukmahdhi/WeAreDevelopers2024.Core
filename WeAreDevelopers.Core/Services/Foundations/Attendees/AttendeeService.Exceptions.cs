// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;
using Xeptions;

namespace WeAreDevelopers.Core.Services.Foundations.Attendees
{
    public partial class AttendeeService
    {
        private delegate ValueTask<Attendee> ReturningAttendeeFunction();
        private delegate IQueryable<Attendee> ReturningAttendeesFunction();

        private async ValueTask<Attendee> TryCatch(ReturningAttendeeFunction returningAttendeeFunction)
        {
            try
            {
                return await returningAttendeeFunction();
            }
            catch (NullAttendeeException nullAttendeeException)
            {
                throw CreateAndLogValidationException(nullAttendeeException);
            }
            catch (InvalidAttendeeException invalidAttendeeException)
            {
                throw CreateAndLogValidationException(invalidAttendeeException);
            }
            catch (SqlException sqlException)
            {
                var failedAttendeeStorageException =
                    new FailedAttendeeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedAttendeeStorageException);
            }
            catch (NotFoundAttendeeException notFoundAttendeeException)
            {
                throw CreateAndLogValidationException(notFoundAttendeeException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsAttendeeException =
                    new AlreadyExistsAttendeeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsAttendeeException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidAttendeeReferenceException =
                    new InvalidAttendeeReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidAttendeeReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedAttendeeException = new LockedAttendeeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedAttendeeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedAttendeeStorageException =
                    new FailedAttendeeStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedAttendeeStorageException);
            }
            catch (Exception exception)
            {
                var failedAttendeeServiceException =
                    new FailedAttendeeServiceException(exception);

                throw CreateAndLogServiceException(failedAttendeeServiceException);
            }
        }

        private IQueryable<Attendee> TryCatch(ReturningAttendeesFunction returningAttendeesFunction)
        {
            try
            {
                return returningAttendeesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedAttendeeStorageException =
                    new FailedAttendeeStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedAttendeeStorageException);
            }
            catch (Exception exception)
            {
                var failedAttendeeServiceException =
                    new FailedAttendeeServiceException(exception);

                throw CreateAndLogServiceException(failedAttendeeServiceException);
            }
        }

        private AttendeeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var AttendeeValidationException =
                new AttendeeValidationException(exception);

            this.loggingBroker.LogError(AttendeeValidationException);

            return AttendeeValidationException;
        }

        private AttendeeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var AttendeeDependencyException = new AttendeeDependencyException(exception);
            this.loggingBroker.LogCritical(AttendeeDependencyException);

            return AttendeeDependencyException;
        }

        private AttendeeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var AttendeeDependencyValidationException =
                new AttendeeDependencyValidationException(exception);

            this.loggingBroker.LogError(AttendeeDependencyValidationException);

            return AttendeeDependencyValidationException;
        }

        private AttendeeDependencyException CreateAndLogDependencyException(
            Xeption exception)
        {
            var AttendeeDependencyException = new AttendeeDependencyException(exception);
            this.loggingBroker.LogError(AttendeeDependencyException);

            return AttendeeDependencyException;
        }

        private AttendeeServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var AttendeeServiceException = new AttendeeServiceException(exception);
            this.loggingBroker.LogError(AttendeeServiceException);

            return AttendeeServiceException;
        }
    }
}