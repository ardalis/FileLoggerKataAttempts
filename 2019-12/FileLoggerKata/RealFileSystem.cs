using System;
using System.IO;

namespace FileLoggerKata
{
    public class RealFileSystem : IFileSystem
    {
        public DateTime GetLastAccessTime(string filepath)
        {
            return File.GetLastAccessTime(filepath);
        }
    }
}
