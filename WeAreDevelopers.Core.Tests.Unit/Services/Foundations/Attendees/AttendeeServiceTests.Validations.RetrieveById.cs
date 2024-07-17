using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;
using Xunit;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidAttendeeId = Guid.Empty;

            var invalidAttendeeException =
                new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.Id),
                values: "Id is required");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            // when
            ValueTask<Attendee> retrieveAttendeeByIdTask =
                this.AttendeeService.RetrieveAttendeeByIdAsync(invalidAttendeeId);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    retrieveAttendeeByIdTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfAttendeeIsNotFoundAndLogItAsync()
        {
            //given
            Guid someAttendeeId = Guid.NewGuid();
            Attendee noAttendee = null;

            var notFoundAttendeeException =
                new NotFoundAttendeeException(someAttendeeId);

            var expectedAttendeeValidationException =
                new AttendeeValidationException(notFoundAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noAttendee);

            //when
            ValueTask<Attendee> retrieveAttendeeByIdTask =
                this.AttendeeService.RetrieveAttendeeByIdAsync(someAttendeeId);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    retrieveAttendeeByIdTask.AsTask);

            //then
            actualAttendeeValidationException.Should().BeEquivalentTo(expectedAttendeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}