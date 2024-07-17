using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            SqlException sqlException = GetSqlException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.RemoveAttendeeByIdAsync(randomAttendee.Id);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    addAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(randomAttendee.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someAttendeeId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedAttendeeException =
                new LockedAttendeeException(databaseUpdateConcurrencyException);

            var expectedAttendeeDependencyValidationException =
                new AttendeeDependencyValidationException(lockedAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Attendee> removeAttendeeByIdTask =
                this.AttendeeService.RemoveAttendeeByIdAsync(someAttendeeId);

            AttendeeDependencyValidationException actualAttendeeDependencyValidationException =
                await Assert.ThrowsAsync<AttendeeDependencyValidationException>(
                    removeAttendeeByIdTask.AsTask);

            // then
            actualAttendeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someAttendeeId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Attendee> deleteAttendeeTask =
                this.AttendeeService.RemoveAttendeeByIdAsync(someAttendeeId);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    deleteAttendeeTask.AsTask);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedAttendeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someAttendeeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedAttendeeServiceException =
                new FailedAttendeeServiceException(serviceException);

            var expectedAttendeeServiceException =
                new AttendeeServiceException(failedAttendeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Attendee> removeAttendeeByIdTask =
                this.AttendeeService.RemoveAttendeeByIdAsync(someAttendeeId);

            AttendeeServiceException actualAttendeeServiceException =
                await Assert.ThrowsAsync<AttendeeServiceException>(
                    removeAttendeeByIdTask.AsTask);

            // then
            actualAttendeeServiceException.Should()
                .BeEquivalentTo(expectedAttendeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}