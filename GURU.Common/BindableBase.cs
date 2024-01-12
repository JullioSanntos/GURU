using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GURU.Common
{
    [Serializable]
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public static Dispatcher Dispatcher { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [DebuggerStepThrough]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerStepThrough]
        public void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(value, storage)) return;
            storage = value;
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            RaisePropertyChanged(propertyName);
        }
    }
}
