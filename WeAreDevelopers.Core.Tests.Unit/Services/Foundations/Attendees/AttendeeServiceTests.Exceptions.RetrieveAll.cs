// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedAttendeeStorageException(sqlException);

            var expectedAttendeeDependencyException =
                new AttendeeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAttendees())
                    .Throws(sqlException);

            // when
            Action retrieveAllAttendeesAction = () =>
                this.AttendeeService.RetrieveAllAttendees();

            AttendeeDependencyException actualAttendeeDependencyException =
                Assert.Throws<AttendeeDependencyException>(retrieveAllAttendeesAction);

            // then
            actualAttendeeDependencyException.Should()
                .BeEquivalentTo(expectedAttendeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAttendees(),
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
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);

            var failedAttendeeServiceException =
                new FailedAttendeeServiceException(serviceException);

            var expectedAttendeeServiceException =
                new AttendeeServiceException(failedAttendeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAttendees())
                    .Throws(serviceException);

            // when
            Action retrieveAllAttendeesAction = () =>
                this.AttendeeService.RetrieveAllAttendees();

            AttendeeServiceException actualAttendeeServiceException =
                Assert.Throws<AttendeeServiceException>(retrieveAllAttendeesAction);

            // then
            actualAttendeeServiceException.Should()
                .BeEquivalentTo(expectedAttendeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAttendees(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}