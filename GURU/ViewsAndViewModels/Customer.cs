using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using Telerik.Windows.Data;

namespace GURU.ViewsAndViewModels
{
    public class Customer : BindableBase
    {


        #region Id
        private int _id;

        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        #endregion Id

        #region Name
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region City
        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }
        #endregion City

        #region Cities
        private ObservableCollection<string> _cities;
        public ObservableCollection<string> Cities
        {
            get { return _cities; }
            set { SetProperty(ref _cities, value); }
        }
        #endregion Cities

        #region ZipCode
        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set { SetProperty(ref _zipCode, value); }
        }
        #endregion ZipCode

        #region ZipCodes
        private ObservableCollection<string> _zipCodes;
        public ObservableCollection<string> ZipCodes
        {
            get { return _zipCodes; }
            set { SetProperty(ref _zipCodes, value); }
        }
        #endregion ZipCodes


        public static string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //public static string upperVowels = "AEIOU";
        public static string LowerVowels = "aeiou";
        public static string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        public static string Numbers = "0123456789";

        public static Random RandomNbr = new Random();
        public static List<Customer> GetRandomCustomers(int numberOfCustomers)
        {
            var customers = new List<Customer>();
            for (int i = 0; i < numberOfCustomers; i++)
            {
                var cust = GetRandomCustmr();
                cust.Id = i;
                customers.Add(cust);
            }
            return customers;
        }

        public static AsyncLazy<List<Customer>> GetRandomCustomersAsync(int numberOfCustomers)
        {
            return new AsyncLazy<List<Customer>>(() => GetRandomCustomers(numberOfCustomers));
        }
        public static Customer GetRandomCustmr()
        {
            var cust = new Customer();
            var getName = new Func<int, string>(i => Customer.GetRandomFromString(RandomNbr, new List<string>() { Customer.LowerChars, Customer.LowerVowels }, i));
            var getNumbers = new Func<int, string>(i => Customer.GetRandomFromString(RandomNbr, new List<string>() { Customer.Numbers }, i));
            var getNameVar = new Func<int, int, string>((i1, i2) => getName(RandomNbr.Next(i1, i2)));
            cust.Name = getNameVar(4, 8) + " " + getNameVar(4, 6);
            cust.City = getNameVar(4, 8);
            cust.ZipCode = getNumbers(5);
            return cust;
        }
        public static string GetRandomFromString(Random random, List<string> charLists, int length)
        {
            var rndString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var charList = charLists[i % charLists.Count];
                rndString.Append(charList[random.Next(charList.Length)]);
            }

            return rndString.ToString();
        }

    }
}
