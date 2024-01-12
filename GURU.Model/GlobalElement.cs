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
    public class GlobalElement : BindableEntityBase, IComparable<Element>, IHaveParent<MainModel> 
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

        #region Name
        private string _name;
        public override string Name
        {
            get { return _name; }
            set {
                if (value == string.Empty) SetProperty(ref _name, null);
                else SetProperty(ref _name, value);
                Priority = null; /* reset cache */
            }
        }
        #endregion Name

        #region ElementType
        private ElementType _elementType;
        public ElementType ElementType
        {
            get { return _elementType; }
            set
            {
                if (value != null && value.Name == NullName) SetProperty(ref _elementType, null); // Logical reset
                else SetProperty(ref _elementType, value);
                Priority = null; /* reset cache */
            }
        }
        #endregion ElementType

        #region Initial Condition types
        #region GradedConditionTypesList
        private ExtendedObservableCollection<GradedInitialConditionType> _gradedConditionTypesList;
        public ExtendedObservableCollection<GradedInitialConditionType> GradedConditionTypesList
        {
            get { return _gradedConditionTypesList ?? (GradedConditionTypesList = new ExtendedObservableCollection<GradedInitialConditionType>()); }
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
            newConditions?.ForEach(c => { AvailableGradedConditionTypesList.TryReplace(c); c.PropertyChanged += PropagateConditionTypesChanges; });
            var oldConsitions = e.OldItems?.OfType<GradedInitialConditionType>().ToList();
            oldConsitions?.ForEach(c => { RaisePropertyChanged(nameof(AvailableGradedConditionTypesList)); c.PropertyChanged -= PropagateConditionTypesChanges; });
            RaisePropertyChanged(nameof(Element));
        }

        private void PropagateConditionTypesChanges(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(GlobalElement));
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
            get { return _gradedInitialStressTypesList ?? (GradedInitialStressTypesList = new ExtendedObservableCollection<GradedInitialStressType>()); }
            set
            {
                if (_gradedInitialStressTypesList != null) _gradedInitialStressTypesList.CollectionChanged -= GradedInitialStressTypesList_CollectionChanged;
                SetProperty(ref _gradedInitialStressTypesList, value);
                if (_gradedInitialStressTypesList != null) _gradedInitialStressTypesList.CollectionChanged += GradedInitialStressTypesList_CollectionChanged;
            }
        }

        private void GradedInitialStressTypesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newStressTypes = e.NewItems?.OfType<GradedInitialStressType>().ToList();
            newStressTypes?.ForEach(c => { AvailableGradedStressTypesList.TryReplace(c); c.PropertyChanged += PropagateStressTypesChanges; });
            var oldStressTypes = e.OldItems?.OfType<GradedInitialStressType>().ToList();
            oldStressTypes?.ForEach(c => { RaisePropertyChanged(nameof(AvailableGradedStressTypesList)); c.PropertyChanged -= PropagateStressTypesChanges; });
            RaisePropertyChanged(nameof(GlobalElement));
        }

        private void PropagateStressTypesChanges(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(GlobalElement));
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
            private set { SetProperty(ref _availableGradedStressTypesList, value); }
        }
        #endregion AvailableGradedStressTypesList
        #endregion Initial Stress types

        #region Priority
        private int? _priority;

        public int? Priority
        {
            get
            {
                if (_priority == null) {
                    if (GradedConditionTypesList.Any() == false) Priority = 10;
                    if (string.IsNullOrEmpty(Name) == false && ElementType != null) Priority = 40;
                    if (string.IsNullOrEmpty(Name) == false && ElementType == null) Priority = 30;
                    if (string.IsNullOrEmpty(Name) == true && ElementType != null) Priority = 20;
                    if (string.IsNullOrEmpty(Name) == true && ElementType == null) Priority = 10;
                }

                return _priority;
            }
            set { SetProperty(ref _priority, value); }
        }
        #endregion Priority


        #endregion properties

        #region constructors
        public GlobalElement(MainModel parent = null) { Parent = parent; }
        #endregion constructors

        #region Comparison
        public int CompareTo(Element other)
        {
            // ReSharper disable once StringCompareIsCultureSpecific.1
            return string.Compare(this.ElementType.ToString(), other.ElementType.ToString());
        }
        #endregion Comparison
    }
}
