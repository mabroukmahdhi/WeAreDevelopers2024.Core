// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedAttendeeStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedAttendeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Attendee> retrieveAttendeeByIdTask =
                this.AttendeeService.RetrieveAttendeeByIdAsync(someId);

            AttendeeDependencyException actualAttendeeDependencyException =
                await Assert.ThrowsAsync<AttendeeDependencyException>(
                    retrieveAttendeeByIdTask.AsTask);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedAttendeeServiceException =
                new FailedAttendeeServiceException(serviceException);

            var expectedAttendeeServiceException =
                new AttendeeServiceException(failedAttendeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Attendee> retrieveAttendeeByIdTask =
                this.AttendeeService.RetrieveAttendeeByIdAsync(someId);

            AttendeeServiceException actualAttendeeServiceException =
                await Assert.ThrowsAsync<AttendeeServiceException>(
                    retrieveAttendeeByIdTask.AsTask);

            // then
            actualAttendeeServiceException.Should()
                .BeEquivalentTo(expectedAttendeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

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