using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using Xunit;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldAddAttendeeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Attendee randomAttendee = CreateRandomAttendee(randomDateTimeOffset);
            Attendee inputAttendee = randomAttendee;
            Attendee storageAttendee = inputAttendee;
            Attendee expectedAttendee = storageAttendee.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertAttendeeAsync(inputAttendee))
                    .ReturnsAsync(storageAttendee);

            // when
            Attendee actualAttendee = await this.AttendeeService
                .AddAttendeeAsync(inputAttendee);

            // then
            actualAttendee.Should().BeEquivalentTo(expectedAttendee);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(inputAttendee),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}