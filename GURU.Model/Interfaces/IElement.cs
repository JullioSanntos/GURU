using System;
using System.Collections.ObjectModel;
using GURU.Common;
using GURU.Common.Interfaces;

namespace GURU.Model
{
    public interface IElement : IComparable<Element>, IHaveParent<MainModel>
    {
        string Name { get; set; }
        ElementType ElementType { get; set; }
        ExtendedObservableCollection<GradedInitialConditionType> GradedConditionTypesList { get; set; }
        ObservableCollection<GradedInitialConditionType> AvailableGradedConditionTypesList { get; }
        ExtendedObservableCollection<GradedInitialStressType> GradedInitialStressTypesList { get; set; }
        ObservableCollection<GradedInitialStressType> AvailableGradedStressTypesList { get; set; }
        ExtendedObservableCollection<GradedFailureModeType> GradedFailureModesTypesList { get; set; }
        ObservableCollection<GradedFailureModeType> AvailableGradedFailureModeTypesList { get; set; }
    }
}