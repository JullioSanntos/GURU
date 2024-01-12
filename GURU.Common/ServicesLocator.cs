using System;
using System.Collections.Generic;

namespace GURU.Common
{
    public static class ServicesLocator
    {
        public const string DialogServicesKey = nameof(DialogServicesKey);
        private static Dictionary<string, Func<object, object>> _servicesList = new Dictionary<string, Func<object, object>>();

        public static void RegisterService(string serviceName, Func<object, object> serviceFunction)
        {
            _servicesList.Add(serviceName, serviceFunction);
        }

        public static Func<List<object>, object> GetService(string serviceName)
        {
            if (_servicesList.ContainsKey(serviceName)) return _servicesList[serviceName];
            else return null;
        }
    }
}
