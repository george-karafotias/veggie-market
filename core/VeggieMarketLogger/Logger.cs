using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeggieMarketLogger
{
    public class Logger
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

        public enum LogType
        {
            Info,
            Warning,
            Error,
            Exception
        }
    }
}
