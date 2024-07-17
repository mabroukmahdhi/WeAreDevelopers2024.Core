// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
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
        public async Task ShouldModifyAttendeeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomModifyAttendee(randomDateTimeOffset);
            Attendee inputAttendee = randomAttendee;
            Attendee storageAttendee = inputAttendee.DeepClone();
            storageAttendee.UpdatedDate = randomAttendee.CreatedDate;
            Attendee updatedAttendee = inputAttendee;
            Attendee expectedAttendee = updatedAttendee.DeepClone();
            Guid AttendeeId = inputAttendee.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(AttendeeId))
                    .ReturnsAsync(storageAttendee);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateAttendeeAsync(inputAttendee))
                    .ReturnsAsync(updatedAttendee);

            // when
            Attendee actualAttendee =
                await this.AttendeeService.ModifyAttendeeAsync(inputAttendee);

            // then
            actualAttendee.Should().BeEquivalentTo(expectedAttendee);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(inputAttendee.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(inputAttendee),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}