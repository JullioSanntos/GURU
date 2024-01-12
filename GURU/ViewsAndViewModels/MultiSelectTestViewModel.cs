using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GURU.Common;
using GURU.Common.XAMLExtensions;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class MultiSelectTestViewModel
    {


        #region SelectedObjects
        public ObservableCollection<object> SelectedElements { get; private set; } = new ObservableCollection<object>();
        #endregion SelectedObjects


    }
}
