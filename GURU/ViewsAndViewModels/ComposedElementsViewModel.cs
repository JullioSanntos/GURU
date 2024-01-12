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
    public class ComposedElementsViewModel: BindableBase
    {

        #region SelectedElement
        private Element _selectedElement;
        public Element SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                SetProperty(ref _selectedElement, value);
                RaisePropertyChanged(nameof(AvailableGradedConditionTypesList));
                RaisePropertyChanged(nameof(AvailableGradedStressTypesList));
                RaisePropertyChanged(nameof(AvailableGradedFailureModeTypesList));
            }
        }
        #endregion SelectedElement


        #region AvailableGradedConditionTypesList
        public ObservableCollection<GradedInitialConditionType> AvailableGradedConditionTypesList
        {
            get { return SelectedElement?.AvailableGradedConditionTypesList; }
        }
        #endregion AvailableGradedConditionTypesList

        #region AvailableGradedStressTypesList
        public ObservableCollection<GradedInitialStressType> AvailableGradedStressTypesList
        {
            get { return SelectedElement?.AvailableGradedStressTypesList; }
        }
        #endregion AvailableGradedStressTypesList

        #region AvailableGradedFailureModeTypesList
        public ObservableCollection<GradedFailureModeType> AvailableGradedFailureModeTypesList
        {
            get { return SelectedElement?.AvailableGradedFailureModeTypesList; }
        }
        #endregion AvailableGradedFailureModeTypesList

    }
}
