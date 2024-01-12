using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace GURU.Behaviors
{
    public class SelectedItemsBindingBehavior : Behavior<RadMultiColumnComboBox>
    {
        public RadMultiColumnComboBox ComboBox
        {
            get { return this.AssociatedObject; }
        }

        public IList TargetSelectedItems
        {
            get => (IList)GetValue(TargetSelectedItemsProperty);
            set => SetValue(TargetSelectedItemsProperty, value);
        }

        public static readonly DependencyProperty TargetSelectedItemsProperty = DependencyProperty.Register(
            "TargetSelectedItems",
            typeof(IList),
            typeof(SelectedItemsBindingBehavior),
            new PropertyMetadata(new PropertyChangedCallback(OnTargetSelectedItemsPropertyChanged)));

        private static void OnTargetSelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectedItemsBindingBehavior behavior = d as SelectedItemsBindingBehavior;
            RadMultiColumnComboBox combo = behavior.ComboBox;
            IList selectedItems = e.NewValue as IList;

            if (combo != null && selectedItems != null)
            {
                behavior.UpdateTransfer(selectedItems, combo.SelectedItems);
            }
        }

        private void SubscribeToEvents()
        {
            this.ComboBox.SelectedItems.CollectionChanged += SourceSelectedItems_CollectionChanged;
        }

        private void UnsubscribeFromEvents()
        {
            this.ComboBox.SelectedItems.CollectionChanged -= SourceSelectedItems_CollectionChanged;
        }

        private void SourceSelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTransfer(this.ComboBox.SelectedItems, TargetSelectedItems);
        }

        private ConcurrentQueue<Tuple<IList, IList>> EnqueuedChanges { get; } = new ConcurrentQueue<Tuple<IList, IList>>();

        private void UpdateTransfer(IList source, IList target)
        {
            UnsubscribeFromEvents();
            var changes= new Tuple<IList, IList>(source, target);
            EnqueuedChanges.Enqueue(changes);
            TransferEnqueuedChanges(EnqueuedChanges);
            //Transfer(source, target);

            SubscribeToEvents();
        }

        private void TransferEnqueuedChanges(ConcurrentQueue<Tuple<IList, IList>> enqueuedChanges)
        {
            while (enqueuedChanges.TryDequeue(out var changes))
            {
                Transfer(changes.Item1, changes.Item2);
            }
        }

        public static void Transfer(IList source, IList target)
        {
            if (source == null || target == null) return;

            target.Clear();
            foreach (var o in source)
            {
                target.Add(o);
            }
        }
    }

}
