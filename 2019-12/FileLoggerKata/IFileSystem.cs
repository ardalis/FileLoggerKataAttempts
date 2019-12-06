using System;

namespace FileLoggerKata
{
    public interface IFileSystem
    {
        DateTime GetLastAccessTime(string filepath);
    }
}
