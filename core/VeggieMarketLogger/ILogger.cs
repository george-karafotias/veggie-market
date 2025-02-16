namespace VeggieMarketLogger
{
    public interface ILogger
    {
        void Log(string className, string methodName, string message, LogType logType);
    }
}
