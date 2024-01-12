using System.ComponentModel;

namespace GURU.Common
{
    public class ItemCancelEventArgs : CancelEventArgs
    {
        public object Item { get; set; }

        public ItemCancelEventArgs(object item) { Item = item; }

        public ItemCancelEventArgs() { }

    }
}
