using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Common.Extensions;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;

namespace GURU.Model
{
    public class Element : BindableEntityBase, IElement
    {
        #region properties

        #region Parent
        private MainModel _parent;
        [JsonIgnore]
        public MainModel Parent
        {
            //get { return _parent ?? (_parent = MainModel.Instance); }
            get { return _parent; }
            internal set { SetProperty(ref _parent, value); }
        }
        #endregion Parent

        #region ElementType
        private ElementType _elementType;
        public ElementType ElementType
        {
            get { return _elementType; }
            set { SetProperty(ref _elementType, value); }
        }
        #endregion ElementType

        #region Initial Condition types
        #region GradedConditionTypesList
        private ExtendedObservableCollection<GradedInitialConditionType> _gradedConditionTypesList;
        public ExtendedObservableCollection<GradedInitialConditionType> GradedConditionTypesList
        {
            get { return _gradedConditionTypesList ?? 
                    (GradedConditionTypesList = new ExtendedObservableCollection<GradedInitialConditionType>() { IsValid = (s => s.Grade >= 0 && s.Grade < 6) }); }
            set
            {
                if (_gradedConditionTypesList != null) _gradedConditionTypesList.CollectionChanged -= GradedConditionTypesList_CollectionChanged;
                SetProperty(ref _gradedConditionTypesList, value);
                if (_gradedConditionTypesList != null) _gradedConditionTypesList.CollectionChanged += GradedConditionTypesList_CollectionChanged;
            }
        }

        private void GradedConditionTypesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newConditions = e.NewItems?.OfType<GradedInitialConditionType>().ToList();
            newConditions?.ForEach(c => { AvailableGradedConditionTypesList.TryReplace(c); c.PropertyChanged += PropagateGradedTypesChanges; });
            var oldConsitions = e.OldItems?.OfType<GradedInitialConditionType>().ToList();
            oldConsitions?.ForEach(c => { RaisePropertyChanged(nameof(AvailableGradedConditionTypesList)); c.PropertyChanged -= PropagateGradedTypesChanges; });
            RaisePropertyChanged(nameof(Element));
        }

        private void PropagateGradedTypesChanges(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Element));
        }
        #endregion GradedConditionTypesList

        #region AvailableGradedConditionTypesList
        private ObservableCollection<GradedInitialConditionType> _availableGradedConditionTypesList;
        [JsonIgnore]
        public ObservableCollection<GradedInitialConditionType> AvailableGradedConditionTypesList
        {
            get
            {
                if (_availableGradedConditionTypesList == null)
                {
                    if (Parent == null) return _availableGradedConditionTypesList = new ObservableCollection<GradedInitialConditionType>();
                    var allConditions = Parent.InitialConditionTypes.Select(ic => new GradedInitialConditionType(ic)).ToList();
                    AvailableGradedConditionTypesList = new ObservableCollection<GradedInitialConditionType>(allConditions);
                }

                return _availableGradedConditionTypesList;
            }
            private set { SetProperty(ref _availableGradedConditionTypesList, value); }
        }
        #endregion AvailableGradedConditionTypesList
        #endregion Initial Condition types

        #region Initial Stress types
        #region GradedInitialStressTypesList
        private ExtendedObservableCollection<GradedInitialStressType> _gradedInitialStressTypesList;
        public ExtendedObservableCollection<GradedInitialStressType> GradedInitialStressTypesList
        {
            get { return _gradedInitialStressTypesList ??
                    (GradedInitialStressTypesList = new ExtendedObservableCollection<GradedInitialStressType>() { IsValid = (s => s.Grade >= 0 && s.Grade < 6) }); }
            set
            {
                if (_gradedInitialStressTypesList != null) _gradedInitialStressTypesList.CollectionChanged -= GradedInitialStressTypesList_CollectionChanged;
                SetProperty(ref _gradedInitialStressTypesList, value);
                if (_gradedInitialStressTypesList != null) _gradedInitialStressTypesList.CollectionChanged += GradedInitialStressTypesList_CollectionChanged;
            }
        }

        private void GradedInitialStressTypesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newStress = e.NewItems?.OfType<GradedInitialStressType>().ToList();
            newStress?.ForEach(c => { AvailableGradedStressTypesList.TryReplace(c); c.PropertyChanged += PropagateGradedTypesChanges; });
            var oldConsitions = e.OldItems?.OfType<GradedInitialStressType>().ToList();
            oldConsitions?.ForEach(c => { RaisePropertyChanged(nameof(AvailableGradedStressTypesList)); c.PropertyChanged -= PropagateGradedTypesChanges; });
            RaisePropertyChanged(nameof(Element));

            //if (e.NewItems == null) return;
            //e.NewItems.OfType<GradedInitialStressType>().ToList()
            //    .ForEach(i => AvailableGradedStressTypesList.TryReplace(i));
        }
        #endregion GradedInitialStressTypesList

        #region AvailableGradedStressTypesList
        private ObservableCollection<GradedInitialStressType> _availableGradedStressTypesList;
        [JsonIgnore]
        public ObservableCollection<GradedInitialStressType> AvailableGradedStressTypesList
        {
            get
            {
                if (_availableGradedStressTypesList == null)
                {
                    if (Parent == null) return _availableGradedStressTypesList = new ObservableCollection<GradedInitialStressType>();
                    var allStressTypes = Parent.InitialStressTypes.Select(s => new GradedInitialStressType(s));
                    AvailableGradedStressTypesList = new ObservableCollection<GradedInitialStressType>(allStressTypes);
                }

                return _availableGradedStressTypesList;
            }
            set
            {
                //if (_availableGradedStressTypesList != null) _availableGradedStressTypesList.ToList().ForEach(i => i.PropertyChanged -= GradeStressTypes_PropertyChanged);
                SetProperty(ref _availableGradedStressTypesList, value);
                //if (_availableGradedStressTypesList != null) _availableGradedStressTypesList.ToList().ForEach(i => i.PropertyChanged += GradeStressTypes_PropertyChanged);
            }
        }

        private void GradeStressTypes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var stressType = sender as GradedInitialStressType;
            if (e.PropertyName == nameof(stressType.Grade) && stressType != null)
            {
                if (stressType.IsValid) GradedInitialStressTypesList.TryAdd(stressType);
                else
                {
                    stressType.Grade = 0;
                    GradedInitialStressTypesList.Remove(stressType);
                }
            }

        }

        #endregion AvailableGradedStressTypesList
        #endregion Initial Stress types

        #region Initial Failure Modes types
        #region GradedFailureModesTypesList
        private ExtendedObservableCollection<GradedFailureModeType> _gradedFailureModesTypesList;
        public ExtendedObservableCollection<GradedFailureModeType> GradedFailureModesTypesList
        {
            get { return _gradedFailureModesTypesList ?? (GradedFailureModesTypesList = new ExtendedObservableCollection<GradedFailureModeType>()); }
            set
            {
                if (_gradedFailureModesTypesList != null) _gradedFailureModesTypesList.CollectionChanged -= GradedFailureModesTypesList_CollectionChanged;
                SetProperty(ref _gradedFailureModesTypesList, value);
                if (_gradedFailureModesTypesList != null) _gradedFailureModesTypesList.CollectionChanged += GradedFailureModesTypesList_CollectionChanged;
            }
        }

        private void GradedFailureModesTypesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            e.NewItems.OfType<GradedFailureModeType>().ToList()
                .ForEach(i => AvailableGradedFailureModeTypesList.TryReplace(i));
        }
        #endregion GradedInitialStressTypesList

        #region AvailableFailureModeTypesList
        private ObservableCollection<GradedFailureModeType> _availableGradedFailureModeTypesList;
        [JsonIgnore]
        public ObservableCollection<GradedFailureModeType> AvailableGradedFailureModeTypesList
        {
            get
            {
                if (_availableGradedFailureModeTypesList == null)
                {
                    if (Parent == null) return _availableGradedFailureModeTypesList = new ObservableCollection<GradedFailureModeType>();
                    var allFailureTypes = Parent.FailureModeTypes.Select(f => new GradedFailureModeType(f));
                    AvailableGradedFailureModeTypesList = new ObservableCollection<GradedFailureModeType>(allFailureTypes);
                }

                return _availableGradedFailureModeTypesList;
            }
            set
            {
                //_availableGradedFailureModeTypesList?.ToList().ForEach(ac => ac.PropertyChanged -= AvailGradedFailure_PropertyChanged);
                SetProperty(ref _availableGradedFailureModeTypesList, value);
                //_availableGradedFailureModeTypesList?.ToList().ForEach(ac => ac.PropertyChanged += AvailGradedFailure_PropertyChanged);
            }
        }

        private void AvailGradedFailure_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var availGradedFailure = sender as GradedFailureModeType;
            switch (e.PropertyName)
            {
                case nameof(GradedFailureModeType.Grade):
                    if (availGradedFailure?.IsValid == true) GradedFailureModesTypesList.TryAdd(availGradedFailure);
                    else GradedFailureModesTypesList.Remove(availGradedFailure);
                    break;
            }
        }
        #endregion AvailableFailureModeTypesList
        #endregion Initial Failure Modes types

        #region HasGradedTypes

        public bool HasGradedTypes {
            get { return GradedConditionTypesList.Any(g => g.Grade != 0) || GradedInitialStressTypesList.Any(g => g.Grade != 0) || GradedFailureModesTypesList.Any(g => g.Grade != 0); }
        }

        #endregion HasGradedTypes

        #endregion properties

        #region constructors
        public Element(MainModel parent) { Parent = parent; }
        #endregion constructors

        #region GetClone
        public Element GetClone(MainModel mainModel)
        {
            var elem = new Element(mainModel);
            elem.Name = this.Name;
            elem.ElementType = this.ElementType;
            elem.Description = this.Description;
            elem.Id = this.Id;
            this.GradedConditionTypesList.ToList().ForEach(c => elem.GradedConditionTypesList.Add(c.GetClone(null)));
            this.GradedFailureModesTypesList.ToList().ForEach(c => elem.GradedFailureModesTypesList.Add(c.GetClone(null)));
            this.GradedInitialStressTypesList.ToList().ForEach(c => elem.GradedInitialStressTypesList.Add(c.GetClone(null)));

            return elem;
        }
        #endregion GetClone

        #region Comparison
        public int CompareTo(Element other)
        {
            // ReSharper disable once StringCompareIsCultureSpecific.1
            return string.Compare(this.Name, other.Name);
        }
        #endregion Comparison
    }
}
