using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static bool TryAdd<T>(this ObservableCollection<T> collection, T item)
        {
            if (collection.Contains(item)) return false;

            collection.Add(item);
            return true;
        }

        public static bool TryRemove<T>(this ObservableCollection<T> collection, T item)
        {
            if (collection.Contains(item) == false) return true;

            collection.Remove(item);
            return true;
        }

        public static bool TryReplace<T>(this ObservableCollection<T> collection, T replacingItem)
        {
            var isReplaced = false;

            var removingItem = collection.Where(i => object.Equals(i, replacingItem)).FirstOrDefault();
            if (removingItem != null) {
                collection.TryRemove(removingItem);
                isReplaced = true;
            }

            collection.TryAdd(replacingItem);

            return isReplaced;
        }

        public static bool TryAddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            bool allItemsAdded = true;
            foreach (T item in items) {
                var addedResult = collection.TryAdd(item);
                if (addedResult == false) allItemsAdded = false;
            }

            return allItemsAdded;
        }

        public static bool TryRemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            bool allItemsRemoved = true;
            foreach (T item in items)
            {
                var removedResult = collection.TryRemove(item);
                if (removedResult == false) allItemsRemoved = false;
            }

            return allItemsRemoved;
        }
    }
}
