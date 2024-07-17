using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using WeAreDevelopers.Core.Brokers.DateTimes;
using WeAreDevelopers.Core.Brokers.Loggings;
using WeAreDevelopers.Core.Brokers.Storages;
using WeAreDevelopers.Core.Models.Attendees;
using WeAreDevelopers.Core.Services.Foundations.Attendees;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;
using System.Linq;

namespace WeAreDevelopers.Core.Tests.Unit.Services.Foundations.Attendees
{
    public partial class AttendeeServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IAttendeeService AttendeeService;

        public AttendeeServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.AttendeeService = new AttendeeService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

         private static IQueryable<Attendee> CreateRandomAttendees()
        {
            return CreateAttendeeFiller(GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Attendee CreateRandomModifyAttendee(DateTimeOffset dates)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Attendee randomAttendee = CreateRandomAttendee(dates);

            randomAttendee.CreatedDate =
                randomAttendee.CreatedDate.AddDays(randomDaysInPast);

            return randomAttendee;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Attendee CreateRandomAttendee() =>
            CreateAttendeeFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Attendee CreateRandomAttendee(DateTimeOffset dateTimeOffset) =>
            CreateAttendeeFiller(dateTimeOffset).Create();

        private static Filler<Attendee> CreateAttendeeFiller(DateTimeOffset dateTimeOffset)
        {
            var filler = new Filler<Attendee>();

            return filler;
        }
    }
}