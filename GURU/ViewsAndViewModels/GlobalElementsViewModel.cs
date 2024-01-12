using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class GlobalElementsViewModel: BindableBase
    {

        #region SelectedGlobalElement
        private GlobalElement _selectedGlobalElement;
        public GlobalElement SelectedGlobalElement
        {
            get { return _selectedGlobalElement; }
            set
            {
                SetProperty(ref _selectedGlobalElement, value);
                if (_selectedGlobalElement == null) return;
                RaisePropertyChanged(nameof(AvailableGradedConditionTypesList));
                RaisePropertyChanged(nameof(AvailableGradedStressTypesList));
                //RaisePropertyChanged(nameof(AvailableGradedFailureModeTypesList));
            }
        }
        #endregion SelectedGlobalElement


        #region AvailableGradedConditionTypesList
        public ObservableCollection<GradedInitialConditionType> AvailableGradedConditionTypesList
        {
            get { return SelectedGlobalElement?.AvailableGradedConditionTypesList; }
        }
        #endregion AvailableGradedConditionTypesList

        #region AvailableGradedStressTypesList
        public ObservableCollection<GradedInitialStressType> AvailableGradedStressTypesList
        {
            get { return SelectedGlobalElement?.AvailableGradedStressTypesList; }
        }
        #endregion AvailableGradedStressTypesList

        //#region AvailableGradedFailureModeTypesList
        //public ObservableCollection<GradedFailureModeType> AvailableGradedFailureModeTypesList
        //{
        //    get { return SelectedGlobalElement?.AvailableGradedFailureModeTypesList; }
        //}
        //#endregion AvailableGradedFailureModeTypesList

    }
}
