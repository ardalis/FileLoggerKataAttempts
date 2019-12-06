using System;
using System.IO;

namespace FileLoggerKata
{
    public interface ISystemClock
    {
        DateTime Today { get; }
    }

    public class SystemClock : ISystemClock
    {
        public DateTime Today => DateTime.Today;
    }

    public class FileLogger
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISystemClock _systemClock;

        public DateTime TheDate { get; set; }

        public FileLogger() : this(DateTime.Today)
        {
        }

        public FileLogger(DateTime theDate) : this(theDate, new RealFileSystem())
        {
        }
        public FileLogger(DateTime theDate,
    IFileSystem fileSystem) : this(theDate, fileSystem, new SystemClock())
        {
        }


        public FileLogger(DateTime theDate, 
            IFileSystem fileSystem,
            ISystemClock systemClock)
        {
            TheDate = theDate;
            _fileSystem = fileSystem;
            _systemClock = systemClock;
        }

        public void Log(string message)
        {
            string filename = "";
            if(TheDate.DayOfWeek==DayOfWeek.Saturday ||
                TheDate.DayOfWeek==DayOfWeek.Sunday)
            {
                filename = "weekend.txt";
                var accessTime = _fileSystem.GetLastAccessTime(filename);
                if (accessTime < _systemClock.Today.AddDays(-5))
                {
                    // Saturday of access time
                    if(accessTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        accessTime = accessTime.AddDays(-1);
                    }
                    if (File.Exists("weekend.txt"))
                    {
                        File.Move(filename, $"weekend-{accessTime.ToString("yyyyMMdd")}.txt");
                    }
                }
            }
            else
            {
                filename = $"{TheDate.ToString("yyyyMMdd")}.txt";
            }
            File.AppendAllText(filename,  message + Environment.NewLine);
        }
    }
}
