using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using GURU.Common;
using GURU.Common.Log;

namespace GURU.ViewsAndViewModels
{
    public class LogViewModel : BindableBase/*, IAsyncInitialization*/
    {

        #region fields
        private string TestData = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum";
        private List<string> words;
        private int maxword;
        private int index;

        private System.Threading.Timer Timer;
        private System.Random random;

        #endregion fields

        #region properties
        //#region LogEntries
        //private ObservableCollection<LogEntry> _logEntries;

        //public ObservableCollection<LogEntry> LogEntries
        //{
        //    get { return _logEntries ?? (LogEntries = new ObservableCollection<LogEntry>()); }
        //    set { SetProperty(ref _logEntries, value); }
        //}
        //#endregion LogEntries
        #endregion properties

        #region constructors
        #endregion constructors

        #region methods


        private Action _insertLogEntry;
        public Action InsertLogEntry
        {
            get { return _insertLogEntry ?? (_insertLogEntry = new Action(() => Logger.Instance.LogEntries.Add(GetRandomEntry()))); }
            set { _insertLogEntry = value; }
        }

        public async Task InitializeAsync()
        {
            random = new Random();
            words = TestData.Split(' ').ToList();
            maxword = words.Count - 1;

            await Task.Run(() => Enumerable.Range(0, 200000).ToList()
                .ForEach(x => Dispatcher.BeginInvoke(InsertLogEntry))
            );

            Timer = new Timer(x => Dispatcher.BeginInvoke(InsertLogEntry), null, 1000, 10);

        }
        //private void AddRandomEntry()
        //{
        //    Dispatcher.BeginInvoke(InsertLogEntry);
        //}

        private LogEntry GetRandomEntry()
        {
            if (random.Next(1, 10) > 1)
            {
                return new LogEntry()
                {
                    Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
                        .Select(x => words[random.Next(0, maxword)])),
                };
            }

            return new CollapsibleLogEntry()
            {
                Message = string.Join(" ", Enumerable.Range(5, random.Next(10, 50))
                    .Select(x => words[random.Next(0, maxword)])),
                Contents = Enumerable.Range(5, random.Next(5, 10))
                    .Select(i => GetRandomEntry())
                    .ToList()
            };


            #endregion methods

        }

        #region IAsyncInitialization
        //#region Customers
        //private readonly AsyncLazy<ObservableCollection<Customer>> _customersAsyncLazy
        //    = new AsyncLazy<ObservableCollection<Customer>>(() => new ObservableCollection<Customer>(Customer.GetRandomCustomers(1000 * 1000 * 5)));
        //private ObservableCollection<Customer> _customers ;

        //public ObservableCollection<Customer> Customers
        //{
        //    //get { return _customers ?? (_customers = new ObservableCollection<Customer>(Customer.GetRandomCustomers(1000 * 1000 * 5))); }
        //    //get { return _customers; }
        //    get
        //    {
        //        if (_customers == null)
        //        {
        //            Console.WriteLine(@"null _customers");
        //            _customers = new ObservableCollection<Customer>(Customer.GetRandomCustomers(1000 * 1000 * 5));
        //        }

        //        return _customers;
        //    }
        //    set { SetProperty(ref _customers, value); }
        //} 
        //#endregion Customers


        //public async Task InitializeAsync()
        //{
        //    Customers = await _customersAsyncLazy;
        //    Console.WriteLine(@"Init completed");
        //    Console.WriteLine(Customers.Count + @" customers");
        //}
        #endregion IAsyncInitialization

    }
}
