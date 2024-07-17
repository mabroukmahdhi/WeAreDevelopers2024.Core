// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfAttendeeIsNullAndLogItAsync()
        {
            // given
            Attendee nullAttendee = null;

            var nullAttendeeException =
                new NullAttendeeException();

            var expectedAttendeeValidationException =
                new AttendeeValidationException(nullAttendeeException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(nullAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(() =>
                    addAttendeeTask.AsTask());

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfAttendeeIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidAttendee = new Attendee
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidAttendeeException =
                new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.Id),
                values: "Id is required");

            //invalidAttendeeException.AddData(
            //    key: nameof(Attendee.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Attendee model

            invalidAttendeeException.AddData(
                key: nameof(Attendee.CreatedDate),
                values: "Date is required");

            invalidAttendeeException.AddData(
                key: nameof(Attendee.CreatedByUserId),
                values: "Id is required");

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedDate),
                values: "Date is required");

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedByUserId),
                values: "Id is required");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(() =>
                    addAttendeeTask.AsTask());

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee;

            invalidAttendee.UpdatedDate =
                invalidAttendee.CreatedDate.AddDays(randomNumber);

            var invalidAttendeeException = new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedDate),
                values: $"Date is not the same as {nameof(Attendee.CreatedDate)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(() =>
                    addAttendeeTask.AsTask());

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee;
            invalidAttendee.UpdatedByUserId = Guid.NewGuid();

            var invalidAttendeeException =
                new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Attendee.CreatedByUserId)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> addAttendeeTask =
                this.AttendeeService.AddAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(() =>
                    addAttendeeTask.AsTask());

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}