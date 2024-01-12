using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    public class Interface : BindableEntityBase, IHaveParent<MainModel>
    {
        #region Properties

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
        public new string Name
        {
            get
            {
                if (Element1?.Name == null && Element2?.Name == null) return _name;
                else return Element1?.Name + "_" + Element2?.Name;
            }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Element1
        private Element _element1;
        public Element Element1
        {
            get { return _element1; }
            set
            {
                if (this._element1 != null ) this._element1.PropertyChanged -= Element1_PropertyChanged;

                if (value == NoneList.First()) {
                    SetProperty(ref _element1, null);
                    Element2 = null;
                }
                else SetProperty(ref _element1, value);
                if (this._element1 != null) this._element1.PropertyChanged += Element1_PropertyChanged;

            }
        }
        #endregion Element1

        #region Element2
        private Element _element2;

        public Element Element2
        {
            get { return _element2; }
            set
            {
                if (this._element2 != null) this._element2.PropertyChanged -= Element2_PropertyChanged;

                if (value == NoneList.First())
                {
                    SetProperty(ref _element2, null);
                    Element1 = null;
                }
                else SetProperty(ref _element2, value);

                if (this._element2 != null) this._element2.PropertyChanged += Element2_PropertyChanged;

            }
        }
        #endregion Element2

        //#region FailureModeType
        //private FailureModeType _failureModeType;
        //public FailureModeType FailureModeType
        //{
        //    get { return _failureModeType; }
        //    set { SetProperty(ref _failureModeType, value); }
        //}
        //#endregion FailureModeType


        #region GradedFailureModesTypesList
        private ExtendedObservableCollection<GradedFailureModeType> _gradedFailureModesTypesList;
        public ExtendedObservableCollection<GradedFailureModeType> GradedFailureModesTypesList
        {
            get { return _gradedFailureModesTypesList ?? (GradedFailureModesTypesList = new ExtendedObservableCollection<GradedFailureModeType>()); }
            set {
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
                    var allFailureTypes = Parent.FailureModeTypes.Select(f => new GradedFailureModeType(f));
                    AvailableGradedFailureModeTypesList = new ObservableCollection<GradedFailureModeType>(allFailureTypes);
                }

                return _availableGradedFailureModeTypesList;
            }
            set { SetProperty(ref _availableGradedFailureModeTypesList, value); }
        }
        #endregion AvailableFailureModeTypesList

        #region Element1Elements
        private IEnumerable<Element> _element1Elements;
        [JsonIgnore]
        public IEnumerable<Element> Element1Elements
        {
            get
            {
                if (_element1Elements != null && Parent.AllAvailableInterfaces != null) return _element1Elements;

                var result = GetAllAvailableInterfaces();
                _element1Elements = result.Select(t => t.Item1).Distinct().OrderBy(e => e.Name);
                _element1Elements = NoneList.Union(_element1Elements);
                return _element1Elements;
            }
        }
        #endregion Element1Elements

        #region Element2Elements
        private IEnumerable<Element> _element2Elements;
        [JsonIgnore]
        public IEnumerable<Element> Element2Elements
        {
            get {
                if (_element2Elements != null && Parent.AllAvailableInterfaces != null) return _element2Elements;

                var result = GetAllAvailableInterfaces();
                _element2Elements = result.Select(t => t.Item2).Distinct().OrderBy(e => e.Name);
                _element2Elements = NoneList.Union(_element2Elements);
                return _element2Elements;
            }
        }
        #endregion Element2Elements

        #endregion Properties

        //readonly List<Element> NoneList = new List<Element>() { new Element() { Name = string.Empty } };

        private List<Element> _noneList;

        private List<Element> NoneList
        {
            get
            {
                if (_noneList == null)
                    _noneList = new List<Element>() { new Element(Parent) { Name = string.Empty } };

                return _noneList;
            }
        }


        #region Constructors

        public Interface(MainModel parent = null)
        {
            this.PropertyChanged += Interface_PropertyChanged;
            Parent = parent;
        }

        private void Element2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Name));
        }

        private void Element1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(Name));
        }

        private void Interface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Element1):
                    RaisePropertyChanged(nameof(Name));
                    _element2Elements = null; //reset cache
                    RaisePropertyChanged(nameof(Element2Elements));
                    break;
                case nameof(Element2):
                    RaisePropertyChanged(nameof(Name));
                    _element1Elements = null; //reset cache
                    RaisePropertyChanged(nameof(Element1Elements));
                    break;
            }
        }
        #endregion Constructors

        #region methods
        public IEnumerable<Tuple<Element, Element, string>> GetAllAvailableInterfaces()
        {
            IEnumerable<Tuple<Element, Element, string>> result;

            if (Element1 == null && Element2 == null) // if record was just created..
                result = Parent.AllAvailableInterfaces;
            else if (Element1 != null && Element2 == null) // .. or if it is a work-in-progress..
            {
                result = Parent.AllAvailableInterfaces
                    // filter by Element already selected
                    .Where(t => t.Item1 == Element1)
                    // remove the possibility of item2 being equal to item1
                    .Where(t => t.Item2 != Element1);
            }
            else if (Element1 == null && Element2 != null) // .. or if it still is a work-in-progress..
            {
                result = Parent.AllAvailableInterfaces
                    // filter by Element already selected
                    .Where(t => t.Item2 == Element2)
                    // remove the possibility of item2 being equal to item1
                    .Where(t => t.Item1 != Element2);
            }
            else // .. or if it is completed
            {
                result = Parent.AllPossibleInterfaces.Where(i => i.Item1 != Element2 && i.Item2 != Element1);
            }

            return result;
        }
        #endregion methods

    }
}
