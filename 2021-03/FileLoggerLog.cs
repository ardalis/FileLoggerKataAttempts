using Moq;
using System;
using System.IO;
using Xunit;

namespace FileLoggerAttempt
{
    [Collection("Sequential")]
    public class FileLogger_Log
    {
        private FileLogger _logger;
        private string _testMessage = Guid.NewGuid().ToString();

        public FileLogger_Log()
        {
            _logger =  new FileLogger(new SystemDateTime(), new DefaultFileNameStrategy());
        }
        private string GetFileName()
        {
            string dateString = DateTime.Today.ToString("yyyy-MM-dd");

            return $"log{dateString}.txt";
        }

        [Fact]
        public void AppendsMessageToLogFile()
        {
            _logger.Log(_testMessage);

            var fileContents = File.ReadAllText(GetFileName());

            Assert.Contains(_testMessage, fileContents);
        }

        [Fact]
        public void AppendsMessageToLogFileWithCurrentTimePrefix()
        {
            string dateString = DateTime.Today.ToString("yyyy-MM-dd");

            _logger.Log(_testMessage);

            var fileContents = File.ReadAllText(GetFileName());

            Assert.Contains(dateString, fileContents);
        }

        [Fact]
        public void AppendsMessageToLogFileNamedForToday()
        {
            string expectedfileName = GetFileName();

            _logger.Log(_testMessage);

            Assert.True(File.Exists(expectedfileName));
        }

        [Fact]
        public void CreatesNewFileOnNewDay()
        {
            DateTime firstDate = new DateTime(2021, 03, 19, 23, 59, 59);
            DateTime secondDate = firstDate.AddMinutes(1);
            string expectedfileName = "log2021-03-20.txt";

            var fakeDateTime = new FakeDateTime();

            var logger = new FileLogger(fakeDateTime, new DefaultFileNameStrategy());
            fakeDateTime.Now = firstDate;
            logger.Log("foo"); // firstDate

            fakeDateTime.Now = secondDate;
            logger.Log(_testMessage); // secondDate

            Assert.True(File.Exists(expectedfileName));
        }

        [Fact]
        public void CreatesNewFileOnNewDayWithMocks()
        {
            DateTime firstDate = new DateTime(2021, 03, 19, 23, 59, 59);
            DateTime secondDate = firstDate.AddMinutes(1);
            string expectedfileName = "log2021-03-20.txt";

            //var fakeDateTime = new FakeDateTime();
            var mockDateTime = new Mock<IDateTime>();

            var logger = new FileLogger(mockDateTime.Object, new DefaultFileNameStrategy());
            //fakeDateTime.Now = firstDate;
            mockDateTime.Setup(x => x.Now).Returns(firstDate);
            logger.Log("foo"); // firstDate

            //fakeDateTime.Now = secondDate;
            mockDateTime.Setup(x => x.Now).Returns(secondDate);

            logger.Log(_testMessage); // secondDate

            Assert.True(File.Exists(expectedfileName));

            mockDateTime.Verify(x => x.Now, Times.Once);
        }

        [Theory]
        [InlineData(2021, 3, 20)]
        [InlineData(2021, 3, 21)]
        public void LogsToWeekendFileOnWeekend(int year, int month, int day)
        {
            DateTime weekendDate = new DateTime(year, month, day);

            var fakeDateTime = new FakeDateTime();

            var logger = new FileLogger(fakeDateTime, new DefaultFileNameStrategy());
            fakeDateTime.Now = weekendDate;
            logger.Log("something");



            string expectedfileName = "weekend.txt";
            Assert.True(File.Exists(expectedfileName));

            
        }
    }

    public class FakeDateTime : IDateTime
    {
        public DateTime Now { get; set; } = DateTime.Now;
    }
}
