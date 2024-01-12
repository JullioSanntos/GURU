using GURU.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using GURU.Common.Extensions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FMEA.Configuration;
using FMEA.ConfigurationReader;
using GURU.Common.Log;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;
using OfficeOpenXml.DataValidation;
using FMEA.SystemHierarchy;
using GURU.Common.Interfaces;
using System.Diagnostics;
using GURU.Model.Interfaces;

namespace GURU.Model
{
    [Export]
    [JsonConverter(typeof(MainModelConverter))]
    public class MainModel : BindableBase, IDisposable
    {
        #region Properties

        #region Instance

        private static MainModel _instance;
        [JsonIgnore]
        public static MainModel Instance {
            get
            {
                //var parts = Common.DI.Container.Catalog.Parts;
                //var exportDefinitions = parts.SelectMany(p => p.ExportDefinitions);
                //var mainModelExportMetadata = exportDefinitions.FirstOrDefault(ed => ed.ContractName.EndsWith(nameof(MainModel)));
                //var creationPolicy = (CreationPolicy)mainModelExportMetadata.Metadata["CreationPolice"] ;
                if (_instance != null) return _instance;

                if (DI.CreationPolicyPerType.Any())
                {
                    CreationPolicy creationPolicy;
                    DI.CreationPolicyPerType.TryGetValue(typeof(MainModel), out creationPolicy);
                    if (creationPolicy == CreationPolicy.NonShared)
                        //return Common.DI.Container.GetExportedValue<MainModel>(); // must be disposed to manufactured a new instance
                        return new MainModel(); //TODO: use MEF here
                }

                _instance = Common.DI.Container?.GetExportedValue<MainModel>();
                if (_instance == null) _instance = new MainModel();


                return _instance;
            } }
        #endregion Instance

        #region Elements
        private ObservableCollection<Element> _elements;
        public ObservableCollection<Element> Elements
        {
            get
            {
                if (_elements == null)
                {
                    _elements = new ExtendedObservableCollection<Element>();
                    _elements.CollectionChanged += _elements_CollectionChanged;
                    RaisePropertyChanged(nameof(Elements));
                }

                return _elements;
            }
        }

        private void _elements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset) RaisePropertyChanged(nameof(Elements));
            var newElements = e.NewItems?.Cast<Element>().ToList();
            newElements?.ForEach(el => { if (el.Parent == null) el.Parent = this; el.PropertyChanged += ElementChanged; });
            var oldElements = e.OldItems?.Cast<Element>().ToList();
            oldElements?.ForEach(el => { el.PropertyChanged -= ElementChanged; });

            RaisePropertyChanged(nameof(IsValid));
            RaisePropertyChanged(nameof(ComposedElements));
        }

        private void ElementChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(ComposedElements));
        }

        #endregion Elements

        #region GlobalElements
        private ObservableCollection<GlobalElement> _globalElements;
        public ObservableCollection<GlobalElement> GlobalElements
        {
            get
            {
                if (_globalElements == null)
                {
                    _globalElements = new ExtendedObservableCollection<GlobalElement>();
                    _globalElements.CollectionChanged += GlobalElements_CollectionChanged;
                    RaisePropertyChanged(nameof(GlobalElements));
                }

                return _globalElements;
            }
        }

        private void GlobalElements_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset) RaisePropertyChanged(nameof(GlobalElements));
            e.NewItems?.Cast<GlobalElement>().ToList().ForEach(ge => { if (ge.Parent == null) ge.Parent = this; });
            var newGlobElems = e.NewItems?.Cast<GlobalElement>().ToList();
            newGlobElems?.ForEach(el => { if (el.Parent == null) el.Parent = this; el.PropertyChanged += GlobalElementChanged; });
            var oldGlobElems = e.OldItems?.Cast<GlobalElement>().ToList();
            oldGlobElems?.ForEach(el => { el.PropertyChanged -= GlobalElementChanged; });

            RaisePropertyChanged(nameof(IsValid));
            RaisePropertyChanged(nameof(ComposedElements));
        }

        private void GlobalElementChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(ComposedElements));
        }

        #endregion GlobalElements

        #region ComposedElements
        [JsonIgnore]
        public ObservableCollection<Element> ComposedElements
        {
            get
            {
                var composedElements = new ObservableCollection<Element>();
                if (Elements.Any() == false) return composedElements;
                //Copy (by cloning) existing Elements to ComposedElements
                Elements.ToList().ForEach(e => composedElements.Add(e.GetClone(this)));

                // Add Conditions from the most granular (Name and ElementType populated) to the most generic (Name and ElementType clear)
                // Do not override existing conditions, even if they have different grades
                // On each GlobalElement...
                var globElementsorderedBySpecificity = GlobalElements.Select((ge, ix) => new { GlobElem = ge, Index = ix }).OrderByDescending(geix => geix.GlobElem.Priority);
                foreach (var gElemIx in globElementsorderedBySpecificity)
                {
                    var gElem = gElemIx.GlobElem;
                    Trace.WriteLine($"------------ global element {gElemIx.Index} (Name: {gElem.Name}  ElemType: {gElem.ElementType?.Name}) -------------------");
                    gElem.GradedConditionTypesList.ToList().ForEach(gc => Trace.WriteLine(gc.ToString()));
                    // ... analyze eacn Element ...
                    foreach (var compdElem in composedElements)
                    {
                        // ApplySettings one Global Condition at a time
                        foreach (var gCond in gElem.GradedConditionTypesList)
                        {
                            var cCond = compdElem.GradedConditionTypesList.FirstOrDefault(c => c.Name == gCond.Name);
                            var clonedGCond = gCond.GetClone(null);
                            Trace.WriteLine($"Apply Global Condition: {clonedGCond.ToString()} ({clonedGCond.GetHashCode()})");
                            ApplySettings(gElem, compdElem, gCond, cCond
                                , () => { if (cCond == null) compdElem.GradedConditionTypesList.Add(clonedGCond); });

                        }

                        //Remove conditions with Grad = 0 after all settings applied
                        var zeroGradedConditionIndexes = compdElem.GradedConditionTypesList.Select((c, i) => new { Cond = c, Ix = i })
                            .Where(cix => cix.Cond.Grade == 0).ToList();
                        zeroGradedConditionIndexes.ForEach(cix => compdElem.GradedConditionTypesList.RemoveAt(cix.Ix));


                        // ApplySettings one Global Stress at a time
                        foreach (var gStress in gElem.GradedInitialStressTypesList)
                        {
                            var cStress = compdElem.GradedInitialStressTypesList.FirstOrDefault(c => c.Name == gStress.Name);
                            ApplySettings(gElem, compdElem, gStress, cStress
                                , () => { if (cStress == null) compdElem.GradedInitialStressTypesList.Add(gStress.GetClone(null)); });
                        }

                        //Remove Stresses with Grad = 0 after all settings applied
                        var zeroGradedStressIndexes = compdElem.GradedInitialStressTypesList.Select((s, i) => new { Stress = s, Ix = i })
                            .Where(six => six.Stress.Grade == 0).ToList();
                        zeroGradedStressIndexes.ForEach(six => compdElem.GradedInitialStressTypesList.RemoveAt(six.Ix));
                    }
                }

                ClearZeroGradeElements(composedElements);

                Trace.WriteLine("------------ returning ComposedElements ---------------");
                return composedElements;

            }
        }
        #endregion ComposedElements


        public void ApplySettings(GlobalElement gElem, IElement compdElem, IGradedEntity gradedGlobElem, IGradedEntity gradedCompElem, Action AddGlogalItem)
        {
            Trace.WriteLine($"===> Applying global Settings to elem: {compdElem.Name}");
            compdElem.GradedConditionTypesList.ToList().ForEach(c => Trace.WriteLine(c.ToString()));

            // Kickback if logically deleted Elements (Grade = 0)
            if (gradedCompElem?.Grade == 0) {
                Trace.WriteLine($"reject global Settings if Element is logically deleted");
                return;
            }

            // Applying global Settings by Name and ElementType
            if (gElem.ElementType != null && string.IsNullOrEmpty(gElem.Name) == false)
            {
                Trace.WriteLine($"Applying global Settings by Name and ElementTyp");
                if (compdElem.ElementType.Name == gElem.ElementType.Name && PatternMatch(gElem.Name, compdElem.Name))
                {
                    if (gradedGlobElem.Grade == 0 && gradedCompElem != null) gradedCompElem.Grade = 0;
                    else AddGlogalItem();
                    compdElem.GradedConditionTypesList.ToList().ForEach(c => Trace.WriteLine(c.ToString()));
                }
                return;
            }

            // Applying global Settings by Name
            if (string.IsNullOrEmpty(gElem.Name) == false)
            {
                Trace.WriteLine($"Applying global Settings by Name");
                if (PatternMatch(gElem.Name, compdElem.Name))
                {
                    if (gradedGlobElem.Grade == 0 && gradedCompElem != null) gradedCompElem.Grade = 0;
                    else AddGlogalItem();
                    compdElem.GradedConditionTypesList.ToList().ForEach(c => Trace.WriteLine(c.ToString()));
                }
                return;
            }

            // Applying global Settings on Matching ElementTypes
            if (gElem.ElementType != null)
            {
                Trace.WriteLine($"Applying global Settings by ElementType");
                if (compdElem.ElementType?.Name == gElem.ElementType.Name)
                {
                    if (gradedGlobElem.Grade == 0 && gradedCompElem != null) gradedCompElem.Grade = 0;
                    else AddGlogalItem();
                    compdElem.GradedConditionTypesList.ToList().ForEach(c => Trace.WriteLine(c.ToString()));
                }
                return;
            }

            // Matching all
            Trace.WriteLine($"Applying non-filtered global Settings");
            if (gradedGlobElem.Grade == 0 && gradedCompElem != null) gradedCompElem.Grade = 0;
            else AddGlogalItem();
            compdElem.GradedConditionTypesList.ToList().ForEach(c => Trace.WriteLine(c.ToString()));
        }

        public bool PatternMatch(string pattern, string testStr)
        {
            if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(testStr)) return true;
            var patternSought = new Regex(pattern);
            var result = patternSought.Match(testStr.Replace("*", ".")).Success;
            return result;
        }

        public void ClearZeroGradeElements(ObservableCollection<Element> composedElements)
        {
            foreach (var compElem in composedElements.ToList())
            {
                //var compElem = composedElements[cElemIx.Index];
                var elem = Elements.FirstOrDefault(e => e.Name == compElem.Name);

                // Delete zero graded Conditions
                var ceConditionsWithZeroGradedConditionsOnElem = from e in elem.GradedConditionTypesList
                                           join ce in compElem.GradedConditionTypesList
                                           on e.Name equals ce.Name
                                           where e.Grade == 0
                                           select ce;
                ceConditionsWithZeroGradedConditionsOnElem.ToList().ForEach(ce => compElem.GradedConditionTypesList.Remove(ce));

                // Delete zero graded Conditions
                var ceStressesWithZeroGradedConditionsOnElem = from e in elem.GradedInitialStressTypesList
                                                                 join ce in compElem.GradedInitialStressTypesList
                                                                 on e.Name equals ce.Name
                                                                 where e.Grade == 0
                                                                 select ce;
                ceStressesWithZeroGradedConditionsOnElem.ToList().ForEach(ce => compElem.GradedInitialStressTypesList.Remove(ce));

                //if (compElem.GradedConditionTypesList.Any() == false && compElem.GradedInitialStressTypesList.Any() == false)
                //    composedElements.Remove(compElem);
            }

        }

        public class IndexedEnt : IComparable 
        {
            public object Entity { get; set; }
            public int Index { get; set; }

            public int CompareTo(object other)
            {
                var that = other as IndexedEnt;
                if (other == null) return 0;
                if (this.Index == that.Index) return 0;
                if (this.Index > that.Index) return -1;
                else return 1;
            }

            public IndexedEnt(object entity, int index)
            {
                Entity = entity;
                Index = index;
            }
        }

        #region Interfaces
        private ObservableCollection<Interface> _interfaces;
        public ObservableCollection<Interface> Interfaces
        {
            get
            {
                if (_interfaces == null)
                {
                    _interfaces = new ObservableCollection<Interface>();
                    _interfaces.CollectionChanged += _interfaces_CollectionChanged;
                    RaisePropertyChanged(nameof(Interfaces));
                }

                return _interfaces;
            }
        }

        private void _interfaces_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset) RaisePropertyChanged(nameof(Interfaces));
            e.NewItems?.Cast<Interface>().ToList().ForEach(I => { if (I.Parent == null) I.Parent = this; });

            RaisePropertyChanged(nameof(IsValid));
            RaisePropertyChanged(nameof(ComposedElements));
        }

        #region SelectedInterface
        private Interface _selectedInterface;
        [JsonIgnore]
        public Interface SelectedInterface
        {
            get { return _selectedInterface; }
            set
            {
                //if (_selectedInterface != null) _selectedInterface.PropertyChanged -= SelectedInterface_PropertyChanged;
                SetProperty(ref _selectedInterface, value);
                //if (_selectedInterface != null) _selectedInterface.PropertyChanged += SelectedInterface_PropertyChanged;
            }
        }
        #endregion SelectedInterface

        // calculation Cache
        private List<Tuple<Element, Element, string>> _allPossibleInterfacesCache;

        // Set "_allPossibleInterfaces" to null to get a fresh calculation
        [JsonIgnore]
        public List<Tuple<Element, Element, string>> AllPossibleInterfaces
        {
            get
            {
                if (_allPossibleInterfacesCache == null)
                {
                    _allPossibleInterfacesCache = new List<Tuple<Element, Element, string>>();
                    //First update
                    UpdateAllPossibleInterfaces(); RaisePropertyChanged(nameof(AllPossibleInterfaces)); 
                    // Subsequent Elements updates will be maintained by following line of code
                    Elements.CollectionChanged += (s, e) => { UpdateAllPossibleInterfaces(); RaisePropertyChanged(nameof(AllPossibleInterfaces)); };
                }

                return _allPossibleInterfacesCache;
            }
        }

        public List<Tuple<Element, Element, string>> UpdateAllPossibleInterfaces()
        {
            AllPossibleInterfaces.Clear();
            if (Elements?.Count > 1)
            {
                var allPossibleInterfacesCache = Elements
                    .SelectMany(e1 => Elements.Select(e2 => new Tuple<Element, Element, string>(e1, e2, calcTupleId(e1, e2))))
                    .Where(t => t.Item1 != t.Item2);
                AllPossibleInterfaces.AddRange(allPossibleInterfacesCache);
            }

            return AllPossibleInterfaces;
        }

        private readonly Func<Element, Element, string> calcTupleId = new Func<Element, Element, string>((e1, e2) => ((e1?.GetHashCode() ?? 0) + (e2?.GetHashCode() ?? 0)).ToString());

        // calculation Cache
        private List<Tuple<Element, Element, string>> _allAvailableInterfacesCache;
        

        // To get a fresh calculation set "AllAvailableInterfacesCache" to null 
        [JsonIgnore]
        public List<Tuple<Element, Element, string>> AllAvailableInterfaces
        {
            get
            {
                if (_allAvailableInterfacesCache == null) _allAvailableInterfacesCache = new List<Tuple<Element, Element, string>>();
                if (Elements?.Any() != true) return _allAvailableInterfacesCache;
                var allExistingInterfaces = Interfaces.Select(i => new Tuple<Element, Element, string>(i.Element1, i.Element2, calcTupleId(i.Element1, i.Element2)));
                var allAvailableInterfacesCache = AllPossibleInterfaces
                    .Where(pi => allExistingInterfaces.Any(ei => pi.Item3 == ei.Item3) == false);
                _allAvailableInterfacesCache.Clear();
                _allAvailableInterfacesCache.AddRange(allAvailableInterfacesCache);

                return _allAvailableInterfacesCache;
            }
        }

        #endregion Interfaces

        private bool _isValidating;

        public bool IsValidating
        {
            get { return _isValidating; }
            set
            {
                _isValidating = value;
                RaisePropertyChanged(nameof(ValidationErrors));
            }
        }


        #region Lookup properties

        #region IsValid
        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                RaisePropertyChanged(nameof(ValidationErrors));
                var isValid = ValidationErrors.Any() == false;
                return ValidationErrors.Any() == false;
                //// if there are no Elements nor Interfaces collection than there is nothing to process and the recordset is not valid
                //if (this.Elements?.Any() == false || this.Interfaces?.Any() == false) return false;

                ////Reject submit if there are Elements that do not have an Element Type
                //if (this.Elements?.Any(e => e.ElementType == null || e.ElementType.Name == null) == true) return false;

                //// Get all Element's that participate in Interfaces using their Identity property
                //var allInterfacedElements = this.Interfaces?.Select(e1 => e1.Element1?.Id).Union(this.Interfaces.Select(e2 => e2.Element2?.Id))
                //    .Where(e => String.IsNullOrEmpty(e) == false)?.ToList();

                //if (allInterfacedElements == null || allInterfacedElements.Any() == false) return false;

                //bool isValid = false;
                //if (Elements != null)
                //{
                //    // find all Elements that do not participate in, at least, one Interface relations.
                //    //var missedElements = allInterfacedElements?.Except(Elements.Select(e => e.Id));
                //    var missedElements = Elements.Select(e => e.Id).Except(allInterfacedElements);
                //    // If there are, at least, one Element left out of relations the recordset is not valid for processing.
                //    isValid = missedElements?.Any() != true;
                //}

                //return isValid;
            }
        }
        #endregion IsValid

        #region Errors

        private List<string> _validationErrors = new List<string>();
        [JsonIgnore]
        public List<string> ValidationErrors
        {
            get
            {
                _validationErrors = new List<string>();
                _validationErrors.AddRange(ValidationRule1());
                _validationErrors.AddRange(ValidationRule2());
                _validationErrors.AddRange(ValidationRule3());

                return _validationErrors;
            }
        }
        #endregion Errors


        #region ElementTypes
        private ObservableCollection<ElementType> _elementTypes;
        [JsonIgnore]
        public ObservableCollection<ElementType> ElementTypes
        {
            get
            {
                if (_elementTypes == null)
                {
                    _elementTypes = new ObservableCollection<ElementType>();
                    try
                    {
                        var elementTypes = MonsterMatrixReader.Configuration.ElementTypes;
                        elementTypes.ForEach(ic => _elementTypes.Add(new ElementType(ic as CElementType)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                return _elementTypes;
            }
            set { SetProperty(ref _elementTypes, value); }
        }
        #endregion ElementTypes

        #region ElementTypes
        private ObservableCollection<ElementType> _globalElementTypes;
        [JsonIgnore]
        public ObservableCollection<ElementType> GlobalElementTypes
        {
            get
            {
                if (_globalElementTypes == null)
                {
                    _globalElementTypes = new ObservableCollection<ElementType>();
                    _globalElementTypes.Add(new ElementType(null));
                    ElementTypes.ToList().ForEach(et => _globalElementTypes.Add(et));
                }

                return _globalElementTypes;
            }
        }
        #endregion ElementTypes

        #region InitialConditionTypes
        private ObservableCollection<InitialConditionType> _initialConditionType;
        [JsonIgnore]
        public ObservableCollection<InitialConditionType> InitialConditionTypes
        {
            get
            {
                if (_initialConditionType == null)
                {
                    _initialConditionType = new ObservableCollection<InitialConditionType>();
                    try
                    {
                        var initialConditionTypes = MonsterMatrixReader.Configuration.InitialConditionTypes;
                        initialConditionTypes.ForEach(ic => _initialConditionType.Add(new InitialConditionType(ic as CConditionType)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }
                return _initialConditionType;
            }
            set { SetProperty(ref _initialConditionType, value); }
        }
        #endregion InitialConditionTypes

        #region InitialStressTypes
        private ObservableCollection<InitialStressType> _initialStressTypes;
        [JsonIgnore]
        public ObservableCollection<InitialStressType> InitialStressTypes
        {
            get
            {
                if (_initialStressTypes == null)
                {
                    _initialStressTypes = new ObservableCollection<InitialStressType>();
                    try
                    {
                        var initialStresstypes = MonsterMatrixReader.Configuration.InitialStresstypes;
                        initialStresstypes.ForEach(ic => _initialStressTypes.Add(new InitialStressType(ic as CStressType)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                return _initialStressTypes;
            }
            set { SetProperty(ref _initialStressTypes, value); }
        }
        #endregion InitialStressTypes

        #region FailureModeTypes
        private ObservableCollection<FailureModeType> _failureModeTypes;
        [JsonIgnore]
        public ObservableCollection<FailureModeType> FailureModeTypes
        {
            get
            {
                if (_failureModeTypes == null)
                {
                    _failureModeTypes = new ObservableCollection<FailureModeType>();
                    try
                    {
                        var failureModeTypes = MonsterMatrixReader.Configuration.FailureModeTypes;
                        //failureModeTypes.ForEach(ic => _failureModeTypes.Add(FailureModeType.FactoryGetInstance(ic as CFailureModeType)));
                        failureModeTypes.ForEach(ic => _failureModeTypes.Add(new FailureModeType(ic as CFailureModeType)));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                return _failureModeTypes;
            }
            set { _failureModeTypes = value; }
        }
        #endregion FailureModeTypes
        #endregion Lookup properties

        #region Configuration properties
        #region AssemblyDirectory
        private static string _assemblyDirectory; 
        public static string AssemblyDirectory {
            get
            {
                try
                {
                    if (_assemblyDirectory == null)
                    {
                        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                        UriBuilder uri = new UriBuilder(codeBase);
                        var path = Uri.UnescapeDataString(uri.Path);
                        _assemblyDirectory = Path.GetDirectoryName(path);
                        Logger.Instance.LogEntries.Add(new LogEntry("Assembly directory: " + _assemblyDirectory));
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry(e));
                    throw;
                }

                return _assemblyDirectory;
            }
        }
        #endregion AssemblyDirectory

        #region ConfigFilePath
        private string  _configFilePath;
        [JsonIgnore]
        public string  ConfigFilePath
        {
            get
            {
                var masterTableName = "MasterTable.xlsx";
                try
                {
                    if (_configFilePath == null)
                    {
                        //var configDirectory = AssemblyDirectory;
                        var configDirectory = AppDataFolderInfo.FullName;
                        var masterTableArray = Properties.Resources.MasterTable_V14;
                        if (masterTableArray == null) throw new Exception("missing " + masterTableName);
                        // ReSharper disable once AssignNullToNotNullAttribute
                        _configFilePath = Path.Combine(configDirectory, masterTableName);
                        if (File.Exists(_configFilePath) == false)
                        {
                            File.WriteAllBytes(_configFilePath, masterTableArray);
                            Logger.Instance.LogEntries.Add(new LogEntry(_configFilePath + " created."));
                        }
                        else Logger.Instance.LogEntries.Add(new LogEntry("Master Table: " + nameof(Properties.Resources.MasterTable_V14)));

                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry(e));
                    throw;
                }


                if (File.Exists(_configFilePath) == false) throw new Exception($"'{masterTableName}' not found in '" + _configFilePath + "' folder");

                return _configFilePath;
            }
            set { _configFilePath = value; }
        }
        #endregion ConfigFilePath

        #region MonsterMatrixReader
        private CExcelMonsterMatrixReaderT _monsterMatrixReader;
        [JsonIgnore]
        public CExcelMonsterMatrixReaderT MonsterMatrixReader
        {
            get
            {
                if (_monsterMatrixReader == null)
                {
                        _monsterMatrixReader = new CExcelMonsterMatrixReaderT(ConfigFilePath, "Total work");
                        if (_monsterMatrixReader != null) Logger.Instance.LogEntries.Add(new LogEntry(ConfigFilePath + " configuration file processed"));
                        else Logger.Instance.LogEntries.Add(new LogEntry(ConfigFilePath + " configuration file processing failed"));
                }

                return _monsterMatrixReader;
            }
        }
        #endregion MonsterMatrixReader
        #endregion Configuration properties

        #region AppDataFolderInfo
        [JsonIgnore] private static DirectoryInfo _appDataFolderInfo;
        public static DirectoryInfo AppDataFolderInfo
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var folderName = appData + @"\Corning Incorporated\GURU";
                _appDataFolderInfo = new DirectoryInfo(folderName);
                if (_appDataFolderInfo.Exists == false) _appDataFolderInfo.Create();
                return _appDataFolderInfo;
            }
        }
        #endregion AppDataFolderInfo

        #endregion Properties

        #region constructors : singleton
        private MainModel() { }

        public static MainModel GetInstance()
        {
            return Instance;
        }
        #endregion constructors : singleton

        #region methods

        #region GetSystemHierarchy
        public CSystemHierarchy GetSystemHierarchy(string rootNode, IConfiguration configuration)
        {
            var systemHierarchy = new CSystemHierarchy(rootNode, "description", configuration);
            Logger.Instance.LogEntries.Add(new LogEntry(nameof(CSystemHierarchy) + " instantiated."));
            systemHierarchy.AddNoElement(rootNode, rootNode + " node", ENodeType.Component, null);
            foreach (var elem in ComposedElements)
            {
                if (elem.ElementType == null || elem.Name == null)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry($"Element '{elem.Name}' has a null ElementType, which is invalied"));
                    continue;
                }
                IElementType elemType = elem.ElementType.SourceConfigurationdObject;
                List<IEntryWithNumber<IStressType>> lstistresstypeInitial =
                    elem.GradedInitialStressTypesList.Select(gs =>
                        new CEntryWithNumber<IStressType>(gs.EntityBase.SourceConfigurationdObject, gs.Grade) as
                            IEntryWithNumber<IStressType>).ToList();
                List<IEntryWithNumber<IConditionType>> lsticonditiontypeInitial =
                    elem.GradedConditionTypesList.Where(gc => gc.Grade != 0).Select(gc =>
                        new CEntryWithNumber<IConditionType>(gc.EntityBase.SourceConfigurationdObject, gc.Grade) as
                            IEntryWithNumber<IConditionType>).ToList();
                List<IEntryWithNumber<IFailureModeType>> lstiFailureModeType =
                    elem.GradedFailureModesTypesList.Select(gf =>
                        new CEntryWithNumber<IFailureModeType>(gf.EntityBase.SourceConfigurationdObject, gf.Grade) as
                            IEntryWithNumber<IFailureModeType>).ToList();

                systemHierarchy.AddElement(elem.Name, elem.Description, elemType, lstistresstypeInitial, lsticonditiontypeInitial, lstiFailureModeType, rootNode);
            }
            Logger.Instance.LogEntries.Add(new LogEntry(nameof(CSystemHierarchy) + " populated with "
                    + Elements.Count + " " + nameof(MainModel.Elements) + " objects."));


            foreach (var intFac in Interfaces)
            {
                List<IEntryWithNumber<IFailureModeType>> lstiFailureModeType =
                    intFac.GradedFailureModesTypesList.Select(gf =>
                        new CEntryWithNumber<IFailureModeType>(gf.EntityBase.SourceConfigurationdObject, gf.Grade) as
                            IEntryWithNumber<IFailureModeType>).ToList();

                systemHierarchy.JoinElementsAndAddItems(intFac.Element1.Name, intFac.Element2.Name, intFac.Description, null, null, lstiFailureModeType);
            }
            Logger.Instance.LogEntries.Add(new LogEntry(nameof(CSystemHierarchy) + " populated with "
                    + Interfaces.Count + " " + nameof(MainModel.Interfaces) + " objects."));



            return systemHierarchy;
        }
        #endregion GetSystemHierarchy

        #region RaiseComposedElements
        public void RaiseComposedElements() {
            //RaisePropertyChanged(nameof(ComposedElements));
        }
        #endregion RaiseComposedElements

        #region ValidationRules
        public List<string> ValidationRule1()
        {
            var errorMessages = new List<string>();

            if (this.Elements?.Any() == false)
                errorMessages.Add("1) No Elements were found, therefore, there is nothing to process");

            return errorMessages;
        }
        public List<string> ValidationRule2()
        {
            var errorMessages = new List<string>();

            if ( this.Interfaces?.Any() == false)
                errorMessages.Add("2) No Interfaces were found, therefore, there is nothing to process");

            return errorMessages;
        }
        public List<string> ValidationRule3()
        {
            var errorMessages = new List<string>();

            //Reject submit if there are Elements that do not have an Element Type
            var ElemsWithmissingTypes = this.Elements?.Where(e => e.ElementType == null || e.ElementType.Name == null).ToList();
            if (ElemsWithmissingTypes.Any())
            {
                errorMessages.Add("3) there are Elements that do not have an Element Type:");
                ElemsWithmissingTypes.ForEach(e => errorMessages.Add($"     Element Name: {e.Name}"));
            }


            return errorMessages;
        }
        public List<string> ValidationRule4()
        {
            var errorMessages = new List<string>();

            // Get all Element's that participate in Interfaces using their Identity property
            var allInterfacedElements = this.Interfaces?.Select(e1 => e1.Element1).Union(this.Interfaces.Select(e2 => e2.Element2))
                .Where(e => String.IsNullOrEmpty(e.Id) == false && e.ElementType != null)?.ToList();

            if (allInterfacedElements == null) return errorMessages;
            errorMessages.Add("There are Elements that do not participate in Interfaces:");
            Elements.Select(e => e).Except(allInterfacedElements).ToList().ForEach(e => errorMessages.Add($"     Element Name: {e.Name}"));

            return errorMessages;
        }
        #endregion ValidationRules


        #region IDispose
        public void Dispose()
        {
            Elements.Clear();
            GlobalElements.Clear();
            Interfaces.Clear();
            //_monsterMatrixReader = null;
            _instance = null;
        }
        #endregion IDispose

        #endregion methods

    }
}
