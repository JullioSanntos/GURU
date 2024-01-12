using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GURU.Common.Interfaces;
using Newtonsoft.Json;

namespace GURU.Common
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {

        #region IsEqual
        private Func<T, T, bool> _isEqual = (i1, i2) => Equals(i1, i2);

        public Func<T, T, bool> IsEqual
        {
            get { return _isEqual; }
            set
            {
                _isEqual = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsEqual)));
            }
        }
        #endregion IsEqual

        #region IsValid
        private Func<T, bool> _isValid = (i1) => true;
        [JsonIgnore]
        public Func<T, bool> IsValid
        {
            get { return _isValid; }
            set
            {
                _isValid = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(_isValid)));
            }
        }
        #endregion IsValid

        public delegate void ItemCancelEventHandler(object sender, ItemCancelEventArgs e);
        public event ItemCancelEventHandler CollectionChanging;

        public ExtendedObservableCollection() : base() { }
        public ExtendedObservableCollection(List<T> list) : base(list) { }
        public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) { }
        public new void Add(T item)
        {
            if (this.Any(i => IsEqual(i, item)) == true) return;
            if (IsValid(item) == false) return;

            var cancelArgs = OnCollectionChanging(item);
            if (cancelArgs.Cancel) return;

            base.Add(item);
        }

        public bool TryAdd(T item)
        {
            var isDuplicated = this.Any(i => IsEqual(i, item));
            var isValid = (item as IValidate)?.IsValid;
            var canAdd = isDuplicated == false && isValid != false;

            var cancelArgs = OnCollectionChanging(item);

            if (canAdd && cancelArgs.Cancel == false) base.Add(item);

            return canAdd;
        }

        public bool TryInsert(int index, T item)
        {
            var isDuplicated = this.Any(i => IsEqual(i, item));
            var isValid = (item as IValidate)?.IsValid;
            var canInsert = isDuplicated == false && isValid != false;

            var cancelArgs = OnCollectionChanging(item);

            var canProceed = canInsert && cancelArgs.Cancel == false;
            if (canProceed) base.Insert(index, item);
            return canProceed;
        }

        public new void Insert(int index, T item)
        {
            var isDuplicated = this.Any(i => IsEqual(i, item));
            var isValid = (item as IValidate)?.IsValid;
            var canInsert = isDuplicated == false && isValid != false;

            var cancelArgs = OnCollectionChanging(item);

            var canProceed = canInsert && cancelArgs.Cancel == false;
            if (canProceed) base.Insert(index, item);
        }


        private CancelEventArgs OnCollectionChanging(T item)
        {
            var itemCancelArgs = new ItemCancelEventArgs(item);
            CollectionChanging?.Invoke(this, itemCancelArgs);
            return itemCancelArgs;
        }

        public new void RemoveItem(int index) { base.RemoveItem(index); }
        public new void RemoveAt(int index) { base.RemoveAt(index); }

        public new void Remove(T item)
        {
            foreach (var i in Items.ToList())
            {
                if (IsEqual(i, item)) base.Remove(i);
            }
        }
    }
}
