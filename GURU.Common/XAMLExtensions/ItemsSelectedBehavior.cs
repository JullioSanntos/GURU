using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace GURU.Common.XAMLExtensions
{
    public class ItemsSelectedBehavior : Behavior<MultiSelector>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems", typeof(IList<object>), typeof(ItemsSelectedBehavior)
            , new FrameworkPropertyMetadata(default(List<object>), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault/*, SelectedObjectsChanged*/));

        public IList<object> SelectedItems
        {
            get { return (IList<object>) GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        //static void SelectedObjectsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        //{

        //}


        protected override void OnAttached()
        {
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null) return;
            foreach (var addedItem in e.AddedItems) { SelectedItems.Add(addedItem); }
            foreach (var removedItem in e.RemovedItems) { SelectedItems.Remove(removedItem); }
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }
    }
}
