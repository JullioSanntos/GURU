using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common.Log
{
    public class Logger
    {
        #region Singleton        
        private static readonly object LoggerLock = new object();
        private static Logger _instance;
        public static Logger Instance {
            get {
                if (_instance != null) return _instance;
                lock (LoggerLock) { return _instance ?? (_instance = new Logger()); }
            }
        }
        private Logger() { }
        #endregion Singleton


        private ObservableCollection<LogEntry> _logEntries;
        public ObservableCollection<LogEntry> LogEntries
        {
            get { return _logEntries ?? (_logEntries = new ObservableCollection<LogEntry>()); }
            set { _logEntries = value; }
        }
    }
}
