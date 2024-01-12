using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using GURU.Common;
using GURU.Common.Interfaces;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GURU.Behaviors
{
    public class DoubleClickSelectionBehavior : Behavior<RadMultiColumnComboBox>
    {
        public RadMultiColumnComboBox ComboBox
        {
            get { return this.AssociatedObject; }
        }

        public RadGridView gridView;

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
        }

        private void AssociatedObject_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            var row = source.ParentOfType<GridViewRow>();
            if (row != null)
            {
                var item = row.DataContext;
                this.gridView = row.ParentOfType<RadGridView>();
                if (item is IValidate validatingItem)
                    { if (validatingItem.IsValid) this.gridView.SelectedItems.Add(item); }
                else
                    this.gridView.SelectedItems.Add(item);
            }
        }
    }
}
