using System;
using System.IO;
using Xunit;

namespace FileLoggerKata
{
    public class FileLogger
    {
        public void Log(string message)
        {
            using var writer = File.AppendText("log.txt");
            writer.WriteLine(message);
        }
    }

    public class FileLoggerLog
    {
        [Fact]
        public void WritesMessageToLogTxt()
        {
            var logger = new FileLogger();
            var msg = Guid.NewGuid().ToString();

            logger.Log(msg);

            var logfile = File.ReadAllText("log.txt");
            Assert.Contains(msg, logfile);

        }
    }
}
