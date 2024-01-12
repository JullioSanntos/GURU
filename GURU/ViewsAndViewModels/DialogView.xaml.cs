using System;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GURU.Common;
using Telerik.Windows.Controls.Wizard;

namespace GURU.ViewsAndViewModels
{
    /// <summary>
    /// Interaction logic for DialogView.xaml
    /// </summary>
    public partial class DialogView : Window
    {
        public DialogViewModel ViewModel { get { return DataContext as DialogViewModel;} }


        public DialogView(DialogViewModel dialogViewModel)
        {
            InitializeComponent();
            DataContext = dialogViewModel;
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.IsVisible):
                    //if (IsVisible == true) this.ShowDialog();
                    if (ViewModel.IsBlocking) this.ShowDialog();
                    else this.Show();
                    break;
                case nameof(ViewModel.CurrentSelection):
                    this.ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                    this.Close();
                    break;
                case nameof(ViewModel.FadeOutSeconds):
                    WindowStyle = WindowStyle.None;
                    AllowsTransparency = true;
                    var fadeOutDuration = new Duration(TimeSpan.FromSeconds(ViewModel.FadeOutSeconds));
                    var animation = new DoubleAnimation() { From = 1.0, To = 0.0, Duration = fadeOutDuration };
                    animation.Completed += (s, ae) => { this.Close(); };
                    this.BeginAnimation(Window.OpacityProperty, animation);
                    break;
            }
        }
    }
}
