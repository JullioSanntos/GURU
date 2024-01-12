﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GURU.Model;
using Telerik.Windows.Controls.GridView;

namespace GURU.ViewsAndViewModels
{
    /// <summary>
    /// Interaction logic for ElementsView.xaml
    /// </summary>
    public partial class ElementsView : UserControl
    {
        public ElementsView()
        {
            InitializeComponent();
        }

        private void ElementsGridView_OnAddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            e.NewObject = new Element(MainModel.Instance);
            e.OwnerGridViewItemsControl.SelectedItem = e.NewObject;
        }
    }
}
