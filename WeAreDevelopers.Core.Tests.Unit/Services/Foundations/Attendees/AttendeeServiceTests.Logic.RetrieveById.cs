// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAttendeeByIdAsync()
        {
            // given
            Attendee randomAttendee = CreateRandomAttendee();
            Attendee inputAttendee = randomAttendee;
            Attendee storageAttendee = randomAttendee;
            Attendee expectedAttendee = storageAttendee.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(inputAttendee.Id))
                    .ReturnsAsync(storageAttendee);

            // when
            Attendee actualAttendee =
                await this.AttendeeService.RetrieveAttendeeByIdAsync(inputAttendee.Id);

            // then
            actualAttendee.Should().BeEquivalentTo(expectedAttendee);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(inputAttendee.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}