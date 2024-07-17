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
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidAttendeeId = Guid.Empty;

            var invalidAttendeeException =
                new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.Id),
                values: "Id is required");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            // when
            ValueTask<Attendee> removeAttendeeByIdTask =
                this.AttendeeService.RemoveAttendeeByIdAsync(invalidAttendeeId);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    removeAttendeeByIdTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}