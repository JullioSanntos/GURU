using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class InterfacesGridViewModel : BindableBase
    {


        #region SelectedElement
        private Interface _selectedInterface;
        public Interface SelectedInterface
        {
            get { return _selectedInterface; }
            set
            {
                SetProperty(ref _selectedInterface, value);
                RaisePropertyChanged(nameof(AvailableGradedFailureModeTypesList));
            }
        }
        #endregion SelectedElement

        #region AvailableGradedFailureModeTypesList
        public ObservableCollection<GradedFailureModeType> AvailableGradedFailureModeTypesList
        {
            get { return SelectedInterface?.AvailableGradedFailureModeTypesList; }
        }
        #endregion AvailableGradedFailureModeTypesList

    }
}
