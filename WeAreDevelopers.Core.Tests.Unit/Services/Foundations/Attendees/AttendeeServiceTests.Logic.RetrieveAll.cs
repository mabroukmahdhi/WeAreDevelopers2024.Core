// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public void ShouldReturnAttendees()
        {
            // given
            IQueryable<Attendee> randomAttendees = CreateRandomAttendees();
            IQueryable<Attendee> storageAttendees = randomAttendees;
            IQueryable<Attendee> expectedAttendees = storageAttendees;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllAttendees())
                    .Returns(storageAttendees);

            // when
            IQueryable<Attendee> actualAttendees =
                this.AttendeeService.RetrieveAllAttendees();

            // then
            actualAttendees.Should().BeEquivalentTo(expectedAttendees);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllAttendees(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}