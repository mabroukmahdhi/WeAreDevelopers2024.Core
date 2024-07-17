using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;
using Xunit;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            SqlException sqlException = GetSqlException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(randomAttendee);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(randomAttendee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Attendee someAttendee = CreateRandomAttendee();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidAttendeeReferenceException =
                new InvalidAttendeeReferenceException(foreignKeyConstraintConflictException);

            AttendeeDependencyValidationException expectedAttendeeDependencyValidationException =
                new AttendeeDependencyValidationException(invalidAttendeeReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(someAttendee);

            AttendeeDependencyValidationException actualAttendeeDependencyValidationException =
                await Assert.ThrowsAsync<AttendeeDependencyValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(someAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedAttendeeDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(someAttendee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            var databaseUpdateException = new DbUpdateException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(databaseUpdateException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(randomAttendee);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(randomAttendee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedAttendeeException =
                new LockedAttendeeException(databaseUpdateConcurrencyException);

            var expectedAttendeeDependencyValidationException =
                new AttendeeDependencyValidationException(lockedAttendeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(randomAttendee);

            AttendeeDependencyValidationException actualAttendeeDependencyValidationException =
                await Assert.ThrowsAsync<AttendeeDependencyValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(randomAttendee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            var serviceException = new Exception();

            var failedAttendeeServiceException =
                new FailedAttendeeServiceException(serviceException);

            var expectedAttendeeServiceException =
                new AttendeeServiceException(failedAttendeeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(randomAttendee);

            AttendeeServiceException actualAttendeeServiceException =
                await Assert.ThrowsAsync<AttendeeServiceException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeServiceException.Should()
                .BeEquivalentTo(expectedAttendeeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(randomAttendee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}