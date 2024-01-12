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
using Telerik.Windows.Controls;

namespace GURU.ViewsAndViewModels
{
    /// <summary>
    /// Interaction logic for MainViewT.xaml
    /// </summary>
    public partial class MainViewT : Window
    {
        public MainViewT()
        {
            StyleManager.ApplicationTheme = new Office2016Theme();
            //StyleManager.ApplicationTheme = new VisualStudio2013Theme();
            InitializeComponent();
        }
    }
}
