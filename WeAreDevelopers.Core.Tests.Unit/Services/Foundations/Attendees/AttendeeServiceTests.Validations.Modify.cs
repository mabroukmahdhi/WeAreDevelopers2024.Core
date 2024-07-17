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
using WeAreDevelopers.Core.Models.Attendees.Exceptions;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAttendeeIsNullAndLogItAsync()
        {
            // given
            Attendee nullAttendee = null;
            var nullAttendeeException = new NullAttendeeException();

            var expectedAttendeeValidationException =
                new AttendeeValidationException(nullAttendeeException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(nullAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfAttendeeIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidAttendee = new Attendee
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidAttendeeException = new InvalidAttendeeException();

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
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Attendee.CreatedDate)}"
                });

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedByUserId),
                values: "Id is required");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            //then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateAttendeeAsync(It.IsAny<Attendee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee;
            var invalidAttendeeException = new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedDate),
                values: $"Date is the same as {nameof(Attendee.CreatedDate)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfAttendeeDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomModifyAttendee(randomDateTimeOffset);
            Attendee nonExistAttendee = randomAttendee;
            Attendee nullAttendee = null;

            var notFoundAttendeeException =
                new NotFoundAttendeeException(nonExistAttendee.Id);

            var expectedAttendeeValidationException =
                new AttendeeValidationException(notFoundAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(nonExistAttendee.Id))
                .ReturnsAsync(nullAttendee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(nonExistAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(nonExistAttendee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomModifyAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee.DeepClone();
            Attendee storageAttendee = invalidAttendee.DeepClone();
            storageAttendee.CreatedDate = storageAttendee.CreatedDate.AddMinutes(randomMinutes);
            storageAttendee.UpdatedDate = storageAttendee.UpdatedDate.AddMinutes(randomMinutes);
            var invalidAttendeeException = new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.CreatedDate),
                values: $"Date is not the same as {nameof(Attendee.CreatedDate)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id))
                .ReturnsAsync(storageAttendee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeValidationException.Should()
                .BeEquivalentTo(expectedAttendeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedAttendeeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserIdDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomModifyAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee.DeepClone();
            Attendee storageAttendee = invalidAttendee.DeepClone();
            invalidAttendee.CreatedByUserId = Guid.NewGuid();
            storageAttendee.UpdatedDate = storageAttendee.CreatedDate;

            var invalidAttendeeException = new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.CreatedByUserId),
                values: $"Id is not the same as {nameof(Attendee.CreatedByUserId)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id))
                .ReturnsAsync(storageAttendee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(invalidAttendee);

            AttendeeValidationException actualAttendeeValidationException =
                await Assert.ThrowsAsync<AttendeeValidationException>(
                    modifyAttendeeTask.AsTask);

            // then
            actualAttendeeValidationException.Should().BeEquivalentTo(expectedAttendeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedAttendeeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Attendee randomAttendee = CreateRandomModifyAttendee(randomDateTimeOffset);
            Attendee invalidAttendee = randomAttendee;
            Attendee storageAttendee = randomAttendee.DeepClone();

            var invalidAttendeeException = new InvalidAttendeeException();

            invalidAttendeeException.AddData(
                key: nameof(Attendee.UpdatedDate),
                values: $"Date is the same as {nameof(Attendee.UpdatedDate)}");

            var expectedAttendeeValidationException =
                new AttendeeValidationException(invalidAttendeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id))
                .ReturnsAsync(storageAttendee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Attendee> modifyAttendeeTask =
                this.AttendeeService.ModifyAttendeeAsync(invalidAttendee);

            // then
            await Assert.ThrowsAsync<AttendeeValidationException>(
                modifyAttendeeTask.AsTask);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedAttendeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAttendeeByIdAsync(invalidAttendee.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}