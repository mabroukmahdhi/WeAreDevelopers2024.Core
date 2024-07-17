// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Attendee someAttendee = CreateRandomAttendee();
            SqlException sqlException = GetSqlException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(someAttendee);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    addAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfAttendeeAlreadyExsitsAndLogItAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            Attendee alreadyExistsAttendee = randomAttendee;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsAttendeeException =
                new AlreadyExistsAttendeeException(duplicateKeyException);

            var expectedAttendeeDependencyValidationException =
                new AttendeeDependencyValidationException(alreadyExistsAttendeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(alreadyExistsAttendee);

            // then
            AttendeeDependencyValidationException actualAttendeeDependencyValidationException =
                await Assert.ThrowsAsync<AttendeeDependencyValidationException>(
                    addAttendeeTask.AsTask);

            actualAttendeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Attendee someAttendee = CreateRandomAttendee();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidAttendeeReferenceException =
                new InvalidAttendeeReferenceException(foreignKeyConstraintConflictException);

            var expectedAttendeeValidationException =
                new AttendeeDependencyValidationException(invalidAttendeeReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(someAttendee);

            // then
            AttendeeDependencyValidationException actualAttendeeDependencyValidationException =
                await Assert.ThrowsAsync<AttendeeDependencyValidationException>(
                    addAttendeeTask.AsTask);

            actualAttendeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(someAttendee),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Attendee someAttendee = CreateRandomAttendee();

            var databaseUpdateException =
                new DbUpdateException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(databaseUpdateException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(someAttendee);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    addAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Attendee someAttendee = CreateRandomAttendee();
            var serviceException = new Exception();

            var failedAttendeeServiceException =
                new FailedAttendeeServiceException(serviceException);

            var expectedAttendeeServiceException =
                new AttendeeServiceException(failedAttendeeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(someAttendee);

            AttendeeServiceException actualAttendeeServiceException =
                await Assert.ThrowsAsync<AttendeeServiceException>(
                    addAttendeeTask.AsTask);

            // then
            actualAttendeeServiceException.Should()
                .BeEquivalentTo(expectedAttendeeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}