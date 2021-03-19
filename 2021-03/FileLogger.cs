using System;
using System.IO;

namespace FileLoggerAttempt
{
    public interface IFileNameStrategy
    {
        string GetFileName(DateTime currentTime);
    }
    public class DefaultFileNameStrategy : IFileNameStrategy
    {
        public string GetFileName(DateTime currentTime)
        {
            if (currentTime.DayOfWeek == DayOfWeek.Saturday ||
                currentTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return "weekend.txt";
            }
            string todayString = currentTime.ToString("yyyy-MM-dd");
            return $"log{todayString}.txt";
        }
    }

    public interface IDateTime
    {
        DateTime Now { get; }
    }

    public class SystemDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }

    public class SomeService
    {
        private readonly FileLogger _fileLogger;

        public SomeService(FileLogger fileLogger)
        {
            _fileLogger = fileLogger;
        }

        public void SomeMethod()
        {
            if(true) // some crazy condition
            _fileLogger.Log("whatever");
            // do stuff I want to test.
        }
    }
    public class FileLogger
    {
        public FileLogger(IDateTime dateTime,
            IFileNameStrategy fileNameStrategy)
        {
            _dateTime = dateTime;
            _fileNameStrategy = fileNameStrategy;
        }

        private static object _lock = new object();
        private readonly IDateTime _dateTime;
        private readonly IFileNameStrategy _fileNameStrategy;

        public void Log(string message)
        {
            string fileName = _fileNameStrategy.GetFileName(_dateTime.Now);
            lock (_lock)
            {
                using var writer = File.AppendText(fileName);

                string dateString = _dateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                writer.WriteLine($"{dateString} {message}");
                writer.Close();
            }
        }
    }
}
