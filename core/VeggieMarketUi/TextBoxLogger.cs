using System;
using System.Windows;
using System.Windows.Controls;
using VeggieMarketLogger;

namespace VeggieMarketUi
{
    public class TextBoxLogger : ILogger
    {
        private TextBox textBox;
        private const int MAX_CHARACTERS = 5000;

        public TextBoxLogger(TextBox textBox)
        {
            this.textBox = textBox;
        }

        public void Log(string className, string methodName, string message, LogType logType)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (textBox.Text.Length > MAX_CHARACTERS)
                {
                    textBox.Clear();
                }

                textBox.AppendText("Class: " + className + ", MethodName: " + methodName + ", Message: " + message + ", Type: " + logType.ToString());
                textBox.AppendText(Environment.NewLine);
                textBox.ScrollToEnd();
            });
        }
    }
}
