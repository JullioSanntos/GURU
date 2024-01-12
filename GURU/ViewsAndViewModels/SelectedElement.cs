using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Common.ModelBase;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class SelectedElement : ElementBase  
    {
        #region MyProperty
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetProperty(ref isSelected, value); }
        }
        #endregion MyProperty
    }
}
