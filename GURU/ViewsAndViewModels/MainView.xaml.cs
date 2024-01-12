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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GURU.Common;
using Telerik.Windows.Controls;

namespace GURU.ViewsAndViewModels
{
    /// <summary>
    /// Interaction logic for MainViewT.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            StyleManager.ApplicationTheme = new Office2016Theme();
            //StyleManager.ApplicationTheme = new VisualStudio2013Theme();
            InitializeComponent();
            this.Loaded += MainView_Loaded;
        }


        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            ServicesLocator.RegisterService(ServicesLocator.DialogServicesKey, GetDialogServicesFactory);
            var mainViewModel = this.DataContext as MainViewModel;
            if (mainViewModel == null) return;
            var saveFileCommand = mainViewModel.SaveCommand;
            var keyBinding = new KeyBinding(saveFileCommand, Key.S, ModifierKeys.Control);
            this.InputBindings.Add(keyBinding);
        }


        #region DialogView
        public DialogView DialogView { get; set; }
        #endregion DialogView

        public DialogViewModel GetDialogServicesFactory(object content)
        {
            var dvm = new DialogViewModel();
            // parse content as a list of objects if it exists
            if (content != null && content is List<object> contentList && contentList.Count > 0)
            {
                dvm.Message = contentList[0] as string;
                if (contentList.Count > 1) dvm.Title = contentList[1] as string;
            }

            DialogView = new DialogView(dvm) {Owner = this};
            //var dialogWindow = new DialogView(dvm) {Owner = this};

            //return dialogWindow;
            return dvm;
        }

    }
}
