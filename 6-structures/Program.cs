using System;

interface ILogger
{
    void Log();
}

interface ILocker
{
    void Log();
}

class FileLogger : ILogger, ILocker
{
    void ILogger.Log()
    {
        Console.WriteLine("Logging to file");
    }

    void ILocker.Log()
    {
        Console.WriteLine("Locking to file");
    }
}

class Program
{
    static void Main(string[] args)
    {
        ILogger logger = new FileLogger();
        logger.Log();  

        ILocker locker = new FileLogger();
        locker.Log();  
    }
}
