using System;

namespace VeggieMarketLogger
{
    public class Logger : ILogger
    {
        private static Logger loggerInstance;

        private Logger()
        {

        }

        public static Logger GetInstance()
        {
            if (loggerInstance == null)
            {
                loggerInstance = new Logger();
            }
            return loggerInstance;
        }

        public void Log(string className, string methodName, string message, LogType logType)
        {
            Console.WriteLine("Class: " + className + ", MethodName: " + methodName + ", Message: " + message + ", Type: " + logType.ToString());
        }
    }
}
