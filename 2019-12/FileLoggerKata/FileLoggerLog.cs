using Moq;
using System;
using System.IO;
using Xunit;

namespace FileLoggerKata
{
    public class FileLoggerLog
    {
        private string GetExpectedFileName()
        {
            return $"{DateTime.Today.ToString("yyyyMMdd")}.txt";
        }

        [Fact]
        public void AppendsMessageToLogDotTxt()
        {
            var logger = new FileLogger();

            string expected = Guid.NewGuid().ToString();
            logger.Log(expected);

            var result = File.ReadAllText(GetExpectedFileName());

            Assert.Contains(expected, result);
        }

        [Fact]
        public void CreatesFileIfItDoesNotExist()
        {
            File.Delete(GetExpectedFileName());
            var logger = new FileLogger();

            string expected = Guid.NewGuid().ToString();
            logger.Log(expected);

            Assert.True(File.Exists(GetExpectedFileName()));
        }

        [Fact]
        public void CreatesFileNamedYYYYMMDDDotTxt()
        {
            string expectedFileName = GetExpectedFileName();
            File.Delete(expectedFileName);
            var logger = new FileLogger();

            string expected = Guid.NewGuid().ToString();
            logger.Log(expected);

            Assert.True(File.Exists(expectedFileName));
        }

        [Fact]
        public void CreatesFileNamedYYYYMMDDGivenDifferentDate()
        {
            string expectedFileName = "20191205.txt"; // Thursday
            File.Delete(expectedFileName);
            var logger = new FileLogger(new DateTime(2019, 12, 5));

            string expected = Guid.NewGuid().ToString();
            logger.Log(expected);

            Assert.True(File.Exists(expectedFileName));
        }

        [Theory]
        [InlineData(2019, 12, 7)]
        [InlineData(2019, 12, 8)]
        public void CreatesFileNamedWeekendDotTxtOnSaturdaySunday(int year, int month, int day)
        {
            string expectedFileName = "weekend.txt";
            File.Delete(expectedFileName);
            var logger = new FileLogger(new DateTime(year, month, day)); // Saturday

            string expected = Guid.NewGuid().ToString();
            logger.Log(expected);

            Assert.True(File.Exists(expectedFileName));
        }

        [Fact]
        public void AppendsToExistingFileNamedWeekendDotTxtOnSunday()
        {
            string expectedFileName = "weekend.txt";
            File.Delete(expectedFileName);

            // log on Saturday
            var logger1 = new FileLogger(new DateTime(2019, 12, 7)); // Saturday
            string expected1 = Guid.NewGuid().ToString();
            logger1.Log(expected1);

            // log on Sunday
            var logger2 = new FileLogger(new DateTime(2019, 12, 8)); // Sunday
            string expected2 = Guid.NewGuid().ToString();
            logger2.Log(expected2);

            var result = File.ReadAllText(expectedFileName);
            Assert.Contains(expected1, result);
            Assert.Contains(expected2, result);
        }

        [Fact]
        public void CreateNewWeekendTxtFileOnNewSaturday()
        {
            var mockFileSystem = new Mock<IFileSystem>();
            var mockSystemClock = new Mock<ISystemClock>();

            // start fresh
            string expectedFileName = "weekend.txt";
            string lastWeekendFilename = "weekend-20191207.txt";
            try
            {
                File.Delete(expectedFileName);
                File.Delete(lastWeekendFilename);
            }
            catch
            {
            }

            // log on 12/8 should work
            mockSystemClock.Setup(c => c.Today).Returns(new DateTime(2019, 12, 8, 9, 0,0));
            var logger1 = new FileLogger(new DateTime(2019, 12, 8), mockFileSystem.Object, mockSystemClock.Object); // Sunday
            string expected1 = Guid.NewGuid().ToString();
            logger1.Log(expected1);
            var result = File.ReadAllText("weekend.txt");
            Assert.Contains(expected1, result);

            // log on second weekend, 12/14
            mockSystemClock.Setup(c => c.Today).Returns(new DateTime(2019, 12, 14, 9, 0, 0));

            // previous weekend.txt was last modified on 12/8
            mockFileSystem.Setup(f => f.GetLastAccessTime("weekend.txt")).Returns(new DateTime(2019, 12, 8));

            var logger2 = new FileLogger(new DateTime(2019, 12, 14), mockFileSystem.Object, mockSystemClock.Object); // Next Saturday
            string expected2 = Guid.NewGuid().ToString();
            logger2.Log(expected2);
            
            result = File.ReadAllText("weekend.txt");

            Assert.DoesNotContain(expected1, result); // logged to renamed file
            Assert.Contains(expected2, result);

            Assert.True(File.Exists(lastWeekendFilename));
            var lastWeekendResult = File.ReadAllText(lastWeekendFilename);
            Assert.Contains(expected1, lastWeekendResult);
        }
    }
}
