using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common.Log
{
    public class LogEntry : BindableBase
    {
        public DateTime DateTime { get {return DateTime.Now;} }

        private static int _index = 0;
        public int Index { get; set; }

        public string Message { get; set; }

        public LogEntry()
        {
           Index = _index++;
        }

        public LogEntry(string message) : this()
        {
            Message = message;
        }

        public LogEntry(Exception exception, [CallerMemberName] string callerName = null)
        {
            Message = (callerName ?? exception.Source) + " threw exception. " + exception.Message;
            foreach (var line in exception.StackTrace.Split(';'))
            {
                Message += Environment.NewLine + line;
            }
        }

        public override string ToString()
        {
            return Index.ToString().PadLeft(6, '0') + $" {DateTime.Now:HH:mm:s} " + Message;
        }
    }
}
