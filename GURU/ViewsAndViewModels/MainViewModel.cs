using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Corning.GenSys.PointsInterface_WCF.PointsServiceRef;
using FMEA.Configuration;
using FMEA.Engine;
using FMEA.ReportGenerator;
using FMEA.SystemHierarchy;
using GURU.Common;
using GURU.Common.Extensions;
using GURU.Common.Interfaces;
using GURU.Common.Log;
using GURU.Common.XAMLExtensions;
using GURU.Model;
using GURU.Model.JSON.Converters;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GURU.ViewsAndViewModels
{
    [Export()]
    public class MainViewModel : BindableBase
    {
        #region fields

        #region FilesFilter
        public const string FilesFilter = "guru files (*.guru)|*.guru|All files (*.*)|.";
        #endregion FilesFilter

        #endregion fields

        #region properties

        #region ActiveView

        private object _activeView;
        public object ActiveView
        {
            get { return _activeView; }
            set { SetProperty(ref _activeView, value); }
        }
        #endregion ActiveView

        #region ComposedElementsGridView
        public ComposedElementsView ComposedElementsGridView { get; } = new ComposedElementsView();
        #endregion ComposedElementsGridView

        #region ElementsGridView
        public ElementsView ElementsGridView { get; } = new ElementsView();
        #endregion ElementsGridView

        #region GlobalElementsGridView
        public GlobalElementsView GlobalElementsGridView { get; } = new GlobalElementsView();
        #endregion ElementsGridView

        #region InterfacesGridView
        public InterfacesGridView InterfacesGridView { get; } = new InterfacesGridView();
        #endregion InterfacesGridView

        #region SplashScreenView
        private SplashScreenView _splashScreenView;

        public SplashScreenView SplashScreenView
        {
            get { return _splashScreenView ?? (_splashScreenView = new SplashScreenView()); }
            set { SetProperty(ref _splashScreenView, value); }
        }
        #endregion SplashScreenView

        #region LogView
        public LogView LogView { get; } = new LogView();
        #endregion LogView

        #region IsBusy
        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }
        #endregion IsBusy


        #region AppDataFolderInfo
        [JsonIgnore] private static DirectoryInfo _appDataFolderInfo;
        public static DirectoryInfo AppDataFolderInfo
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var folderName = appData + @"\Corning\GURU";
                _appDataFolderInfo = new DirectoryInfo(folderName);
                if (_appDataFolderInfo.Exists == false) _appDataFolderInfo.Create();
                return _appDataFolderInfo;
            }
        }
        #endregion AppDataFolderInfo

        #region Filename
        public string Filename { get { return OpenFileInfo?.Name; } }
        public string FilenameForLogging { get { return OpenFileInfo?.Name ?? "..<new file>.. "; } }
        #endregion Filename

        #region OpenFileInfo
        private System.IO.FileInfo _openFileInfo;

        public FileInfo OpenFileInfo
        {
            get { return _openFileInfo; }
            private set
            {
                SetProperty(ref _openFileInfo, value);
                RaisePropertyChanged(nameof(Filename));
                RaisePropertyChanged(nameof(WindowTitleHeader));
            }
        }
        #endregion OpenFileInfo

        #region SystemHierarchy
        private CSystemHierarchy _systemHierarchy;

        public CSystemHierarchy SystemHierarchy
        {
            get { return _systemHierarchy ?? (_systemHierarchy = new CSystemHierarchy("","", new object() as IConfiguration)); }
            set { SetProperty(ref _systemHierarchy, value); }
        }
        #endregion SystemHierarchy

        #region ElementGridViewIsChecked
        private bool _elementGridViewIsChecked;

        public bool ElementGridViewIsChecked
        {
            get { return _elementGridViewIsChecked; ; }
            set { SetProperty(ref _elementGridViewIsChecked, value); }
        }
        #endregion ElementGridViewIsChecked

        #region InterfaceGridViewIsChecked
        private bool _interfaceGridViewIsChecked;

        public bool InterfaceGridViewIsChecked
        {
            get { return _interfaceGridViewIsChecked; }
            set { SetProperty(ref _interfaceGridViewIsChecked, value); }
        }
        #endregion InterfaceGridViewIsChecked

        #region GlobalElementsGridViewIsChecked
        private bool _globalInterfaceGridViewIsChecked;

        public bool GlobalElementsGridViewIsChecked
        {
            get { return _globalInterfaceGridViewIsChecked; }
            set { SetProperty(ref _globalInterfaceGridViewIsChecked, value); }
        }
        #endregion GlobalElementsGridViewIsChecked

        #region ComposedElementsGridViewIsChecked
        private bool _composedElementsGridViewIsChecked;

        public bool ComposedElementsGridViewIsChecked
        {
            get { return _composedElementsGridViewIsChecked; }
            set { SetProperty(ref _composedElementsGridViewIsChecked, value); }
        }
        #endregion ComposedElementsGridViewIsChecked

        #region LogViewIsChecked
        private bool _logViewIsChecked;

        public bool LogViewIsChecked
        {
            get { return _logViewIsChecked; }
            set { SetProperty(ref _logViewIsChecked, value); }
        }
        #endregion LogViewIsChecked

        #region SplashScreenViewModel
        private SplashScreenViewModel _splashScreenViewModel;

        public SplashScreenViewModel SplashScreenViewModel
        {
            get { return _splashScreenViewModel; }
            set { SetProperty(ref _splashScreenViewModel, value); }
        }
        #endregion SplashScreenViewModel

        #region MenuItems
        private ObservableCollection<MenuItem> _menuItems;

        public ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                if (_menuItems == null)
                {
                    _menuItems = new ObservableCollection<MenuItem>();
                    _menuItems.Add(new MenuItem("Home"));
                    _menuItems.Add(new MenuItem("File"));
                    _menuItems.Add(new MenuItem("Elements"));
                    _menuItems.Add(new MenuItem("Interfaces"));
                }
                return _menuItems;
            }
            set { SetProperty(ref _menuItems, value); }
        }
        #endregion MenuItems

        #region IsOperatorTabSelected
        private bool _isOperatorTabSelected;

        public bool IsOperatorTabSelected
        {
            get { return _isOperatorTabSelected; }
            set { SetProperty(ref _isOperatorTabSelected, value); }
        }
        #endregion IsOperatorTabSelected

        #region ViewsInfoDict
        private Dictionary<Type, PropertyInfo> _viewsInfo;
        public Dictionary<Type, PropertyInfo> ViewsInfoDict
        {
            get
            {
                if (_viewsInfo == null)
                {
                    _viewsInfo = new Dictionary<Type, PropertyInfo>();
                    _viewsInfo.Add(ElementsGridView.GetType(), this.GetType().GetProperty(nameof(ElementGridViewIsChecked)));
                    _viewsInfo.Add(InterfacesGridView.GetType(), this.GetType().GetProperty(nameof(InterfaceGridViewIsChecked)));
                    _viewsInfo.Add(GlobalElementsGridView.GetType(), this.GetType().GetProperty(nameof(GlobalElementsGridViewIsChecked)));
                    _viewsInfo.Add(ComposedElementsGridView.GetType(), this.GetType().GetProperty(nameof(ShowComposedElementsViewIsChecked)));
                    _viewsInfo.Add(LogView.GetType(), this.GetType().GetProperty(nameof(LogViewIsChecked)));
                }

                return _viewsInfo;
            }
        }
        #endregion ViewsInfoDict

        #region ViewsList
        private ObservableCollection<object> _viewsList;

        public ObservableCollection<object> ViewsList
        {
            get
            {
                if (_viewsList == null)
                {
                    _viewsList = new ObservableCollection<object>();
                    _viewsList.CollectionChanged += ViewsList_CollectionChanged;
                }
                return _viewsList;
            }
        }

        private void ViewsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
                ViewsInfoDict.Values.ToList().ForEach(pi => pi.SetValue(this, false));

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var view in e.NewItems.Cast<object>())
                {
                    var viewIsCheckedPropInfo = ViewsInfoDict.FirstOrDefault(kv => kv.Key == view.GetType()).Value;
                    if (viewIsCheckedPropInfo == null) continue;
                    viewIsCheckedPropInfo.SetValue(this, true);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (var view in e.OldItems.Cast<object>())
                {
                    var viewIsCheckedPropInfo = ViewsInfoDict.FirstOrDefault(kv => kv.Key == view.GetType()).Value;
                    if (viewIsCheckedPropInfo == null) continue;
                    viewIsCheckedPropInfo.SetValue(this, false);
                }
        }
        #endregion ViewsList

        #region WindowTitleHeader

        public string WindowTitleHeader
        {
            get
            {
                var version = Assembly.GetEntryAssembly().GetName().Version;
                var windowTitleHeader = $"GURU V: {version}";
                if (OpenFileInfo != null) windowTitleHeader = $"{OpenFileInfo.Name} - {windowTitleHeader}";
                return windowTitleHeader;
            }
        }
        #endregion WindowTitleHeader

        #region IsDialogOpened

        private bool isDialogOpened;
        public bool IsDialogOpened
        {
            get { return isDialogOpened; }
            set { SetProperty(ref isDialogOpened, value); }
        }
        #endregion IsDialogOpened

        #region DialogContent
        private object dialogContent;

        public object DialogContent
        {
            get { return dialogContent; }
            set
            {
                SetProperty(ref dialogContent, value);
                IsDialogOpened = dialogContent != null;
            }
        }
        #endregion DialogContent

        #region DialogTextContent
        private object dialogTextContent;

        public object DialogTextContent
        {
            get { return dialogTextContent; }
            set
            {
                SetProperty(ref dialogTextContent, value);
                IsDialogOpened = dialogTextContent != null;
            }
        }
        #endregion DialogContent

        #region DialogsFactory
        private Func<List<object>, object> _dialogsFactory;
        public Func<List<object>, object> DialogsFactory
        {
            get { return _dialogsFactory ?? (_dialogsFactory = ServicesLocator.GetService(ServicesLocator.DialogServicesKey)); }
        }
        #endregion DialogsFactory

        #region ShowComposedElementsViewIsChecked
        private bool _showComposedElementsViewIsChecked;

        public bool ShowComposedElementsViewIsChecked
        {
            get { return _showComposedElementsViewIsChecked; }
            set { SetProperty(ref _showComposedElementsViewIsChecked, value); }
        }
        #endregion ShowComposedElementsViewIsChecked

        #endregion properties

        #region constructors
        public MainViewModel()
        {
            SplashScreenViewModel = SplashScreenView.DataContext as SplashScreenViewModel;
            if (SplashScreenViewModel != null) SplashScreenViewModel.PropertyChanged += SplashScreen_PropertyChanged;
            //ActiveView = SplashScreenView;
            ViewsList.Add(SplashScreenView);

            //FullView = ElementsGridView;
        }


        private void SplashScreen_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SplashScreenViewModel.SelectedFile):
                    if (SplashScreenViewModel.SelectedFile == null) break;
                    if (SplashScreenViewModel.SelectedFile.FileInfo.Exists) { OpenFileInfo = SplashScreenViewModel.SelectedFile.FileInfo; }
                    else
                    {

                        this.PropertyChanged += (s, eventArgs) =>
                        {
                            if (eventArgs.PropertyName == nameof(DialogArgument) && DialogArgument.ToString() == "OK")
                                SplashScreenViewModel.SavedFilesList.Remove(SplashScreenViewModel.SelectedFile);
                        };
                        DialogTextContent = "File not found. Press 'OK' to remove this entry";

                    }

                    if (TryDeserializeGuruFile() == false)
                    {
                        Logger.Instance.LogEntries.Add(new LogEntry($@"Could not open file '{FilenameForLogging}' "));
                        return;
                    }
                    //SplashScreenViewModel.SavedFilesList.TryInsert(0, new SerilzFileInfo(OpenFileInfo.ToString()));
                    var currIndex = SplashScreenViewModel.SavedFilesList.IndexOf(SplashScreenViewModel.SelectedFile);
                    SplashScreenViewModel.SavedFilesList.Move(currIndex, 0); //Move selected FileInfo to the top of the list
                    //ToggleView(ElementsGridView);
                    ViewsList.Add(GlobalElementsGridView);
                    ViewsList.Add(ElementsGridView);
                    ElementGridViewIsChecked = true;
                    IsOperatorTabSelected = true;
                    break;
            }
        }
        #endregion constructors

        #region commands

        #region SplashScreenViewCommand
        public void ActivateSpashScreenView(object arg) { ActiveView = SplashScreenView; }
        public bool CanActivateSpashScreenView(object arg) { return true; }
        #endregion SplashScreenViewCommand

        #region ElementGridViewCommand
        public ICommand ElementGridViewCommand { get { return new RelayCommand((a) => ShowElementGridView(a) ); } }

        public void ShowElementGridView (object arg) {
            if (ViewsList.Contains(ElementsGridView))
            {
                ViewsList.TryRemove(GlobalElementsGridView);
                ViewsList.Remove(ElementsGridView);
            }
            else
            {
                ViewsList.TryAdd(GlobalElementsGridView);
                ViewsList.Add(ElementsGridView);
            }
        }
        #endregion ElementGridViewCommand

        #region InterfacesViewCommand
        public ICommand InterfacesViewCommand { get { return new RelayCommand((a) => ToggleView(InterfacesGridView)); } }
        #endregion InterfacesViewCommand

        #region GlobalElementsViewCommand
        public ICommand GlobalElementsViewCommand { get { return new RelayCommand((a) => ToggleView(GlobalElementsGridView)); } }
        #endregion GlobalElementsViewCommand

        #region ShowCommand
        public ICommand ShowCommand { get { return new RelayCommand((a) => { ToggleView(ComposedElementsGridView); MainModel.Instance.RaiseComposedElements(); }); } }
        #endregion ShowCommand

        #region LogViewCommand
        public ICommand LogViewCommand { get { return new RelayCommand((a) => ToggleView(LogView)); } }
        #endregion LogViewCommand

        #region NewFileCommand
        public ICommand NewFileCommand { get { return new RelayCommand((a) => {NewFileAction();}); } }

        public void NewFileAction()
        {
            MainModel.Instance.Elements.Clear();
            MainModel.Instance.Interfaces.Clear();
            MainModel.Instance.GlobalElements.Clear();
            OpenFileInfo = null;
            //ActiveView = ElementsGridView;
            ViewsList.Clear();
            ViewsList.Add(GlobalElementsGridView);
            ViewsList.Add(ElementsGridView);
            //ElementGridViewIsChecked = true;
        }
        #endregion NewFileCommand

        #region OpenFileCommand
        public ICommand OpenFileCommand { get { return new RelayCommand((a) => { TryOpenGuruFile(); /*ActiveView = ElementsGridView; */}); } }

        public bool TryOpenGuruFile(/*string filename*/)
        {
            try
            {
                IsBusy = true;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = FilesFilter;
                if (ofd.ShowDialog() == false) return false;

                OpenFileInfo = new FileInfo(ofd.FileName);
                if (TryDeserializeGuruFile() == false)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry($@"Could not open file '{FilenameForLogging}' "));
                    return false;
                }
                SplashScreenViewModel.SavedFilesList.TryInsert(0, new SerilzFileInfo(OpenFileInfo.ToString()));
                ShowElementGridView(null);
                ElementGridViewIsChecked = true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogEntries.Add(new LogEntry(e));
                throw;
            }
            finally { IsBusy = false; }

            return true;
        }

        public bool TryDeserializeGuruFile()
        {
            Logger.Instance.LogEntries.Add(new LogEntry() { Message = $@"File '{FilenameForLogging}' is being opened." });
            try
            {
                DeserializeGuruFile();
                Logger.Instance.LogEntries.Add(new LogEntry() { Message = $@"File '{FilenameForLogging}' is open." });
                return true;
            }
            catch (Exception e)
            {
                Logger.Instance.LogEntries.Add(new LogEntry(e));
                Logger.Instance.LogEntries.Add(new LogEntry() { Message = $@"File '{FilenameForLogging}' failed to open." });
                return false;
            }
        }

        public void DeserializeGuruFile()
        {
            ITraceWriter traceWriter = new MemoryTraceWriter();

            if (OpenFileInfo.Exists == false) { OpenFileDiaglog(); }
            var fileContent = File.ReadAllText(OpenFileInfo.ToString());
            ViewsList.Clear();

            try
            {
                var dsrldObj = JsonConvert.DeserializeObject<MainModel>(fileContent,
                    new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                // Assess derialization results
                if (dsrldObj == null || dsrldObj.GetType() != typeof(MainModel))
                {
                    IsBusy = false;
                    throw new ArgumentNullException();
                }

                if (MainModel.Instance.Elements.Where(e => e.Parent == null).Any()) System.Diagnostics.Debugger.Break();
            }
            catch (Exception )
            {
                Console.WriteLine(traceWriter);
                OpenFileDiaglog();
                //var dialogViewModel = DialogsFactory(null) as DialogViewModel;
                //dialogViewModel.Title = "Open file dialog";
                //dialogViewModel.Message = $"{OpenFileInfo.ToString()} deserialization failed.";
                //dialogViewModel.ShowDialog();
                throw;
            }

        }

        public void OpenFileDiaglog() {

            var dialogViewModel = DialogsFactory(null) as DialogViewModel;
            dialogViewModel.Title = "Open file dialog";
            dialogViewModel.Message = $"{OpenFileInfo.ToString()} deserialization failed.";
            dialogViewModel.ShowDialog();
        }

        #endregion OpenFileCommand

        #region SaveCommand
        public ICommand SaveCommand { get { return new RelayCommand((a) => SaveFile()); } }

        public void SaveFile(/*string filename*/)
        {
            
            if (Filename == null) { SaveFileAs(); return; }

            // deserialize by using settings: 
            // Settings: Also, Reference type's objects should not be duplicated. Only one object must exist. For toubleshoting purporses TypeNames are included in the serialized file
            var serializingSettings = new JsonSerializerSettings() {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            };
            var srldObj = JsonConvert.SerializeObject(MainModel.Instance, Formatting.Indented, settings: serializingSettings);
            SplashScreenViewModel.SavedFilesList.TryInsert(0, new SerilzFileInfo(OpenFileInfo?.ToString()));

            File.WriteAllText(OpenFileInfo.ToString(), srldObj);

            if (DialogsFactory(null) is DialogViewModel dialogViewModel)
            {
                dialogViewModel.Message = $"{OpenFileInfo.Name} saved.";
                dialogViewModel.Title = "SAVE dialog";
                dialogViewModel.CommandsList = null;
                dialogViewModel.Show(2.5);
            }
        }

        #endregion SaveCommand

        #region SaveFileAsCommand
        public ICommand SaveFileAsCommand { get { return new RelayCommand((a) => SaveFileAs()); } }
        public void SaveFileAs()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = FilesFilter;

            if (Filename == null) {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sfd.InitialDirectory = path;
                sfd.FileName = "Document1.guru";
            }

            if (sfd.ShowDialog() == true) {
                //OpenFileInfo = new System.IO.FileInfo(sfd.FileName);
                OpenFileInfo = new FileInfo(Path.Combine(sfd.InitialDirectory, sfd.FileName));
                SaveFile();
            }
        }

        #endregion SaveFileAsCommand

        #region SubmitCommand
        public ICommand SubmitCommand { get { return new RelayCommand((a) => TrySubmit() /*, (a) => MainModel.Instance.IsValid*/);} }

        public bool TrySubmit()
        {
            var filename = OpenFileInfo?.Name ?? "<no named file>";

            try
            {
                Logger.Instance.LogEntries.Add(new LogEntry($"{FilenameForLogging} is being processed. "));

                var dialogViewModel = DialogsFactory(null) as DialogViewModel;
                dialogViewModel.Title = "SUBMIT dialog";

                // Do not submit if file is not persisted
                if (OpenFileInfo?.Name == null)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry($"{filename} is not valid for Submit."));

                    //dialogViewModel.Message = $"{filename} is not valid for Submit." + Environment.NewLine + "File must be saved first.";
                    dialogViewModel.Messages = new List<string>();
                    dialogViewModel.Messages.Add($"{filename} is not valid for Submit.");
                    dialogViewModel.Messages.Add($"File must be saved first.");
                    dialogViewModel.ShowDialog();
                    return false;
                }

                // Do not submit if errors are found ...
                if (MainModel.Instance.IsValid == false)
                {
                    Logger.Instance.LogEntries.Add(new LogEntry($"{filename} is not valid for Submit."));
                    dialogViewModel.Messages = MainModel.Instance.ValidationErrors;
                    dialogViewModel.ShowDialog();
                    return false;
                }

                // ... otherwise
                Submit();

                Logger.Instance.LogEntries.Add(new LogEntry($"{FilenameForLogging} processed succesfully. "));
                dialogViewModel.Message = $"{FilenameForLogging} processed succesfully.";
                dialogViewModel.ShowDialog();
                return true;

            }
            catch (Exception e)
            {
                //DialogTextContent = e.Message + Environment.NewLine + $"{FilenameForLogging} processing failed. ";
                var dialogText =$"{FilenameForLogging} processing failed. " + Environment.NewLine + e.Message ;
                Logger.Instance.LogEntries.Add(new LogEntry(e));
                Logger.Instance.LogEntries.Add(new LogEntry(dialogText));
                if (DialogsFactory(null) is DialogViewModel dialogViewModel)
                {
                    dialogViewModel.Message = dialogText;
                    dialogViewModel.Title = "SUBMIT dialog";
                    dialogViewModel.CommandsList = new List<DialogSelection>(){ DialogSelection.OK };
                    dialogViewModel.ShowDialog();
                }

                return false;
            }
        }

        public void Submit()
        {
            Logger.Instance.LogEntries.Add(new LogEntry("'Submit' starts."));

            if (OpenFileInfo == null) SaveFileAs();

            if (OpenFileInfo == null) throw new Exception("Not saved file can't be processed.");

            var configuration = MainModel.Instance.MonsterMatrixReader.Configuration;
            const string rootNode = "TOP";
            var systemHierarchy = MainModel.Instance.GetSystemHierarchy(rootNode, configuration);

            var timestamp = "_" + string.Format("{0:yyMMdd_HHmmss}", DateTime.Now);

            var xmlFilepath = Path.Combine(AppDataFolderInfo.ToString() , OpenFileInfo.Name.Replace(".guru", timestamp + ".xml") );
            Logger.Instance.LogEntries.Add(new LogEntry("Saving file " + xmlFilepath));
            systemHierarchy.SerializeToXML(xmlFilepath);

            List<CElementAnalysed> lstoElementAnalysed = CEngine.PhysicalAnalysis(systemHierarchy.GetAllChildElementBase(rootNode), configuration);
            Logger.Instance.LogEntries.Add(new LogEntry(nameof(CSystemHierarchy) + " physical analysis completed."));
            
            //var reportsPath = OpenFileInfo.ToString().Replace(".guru", "._FlatS.tsv");
            var reportsPath = OpenFileInfo.ToString().Replace(".guru", timestamp + "._FlatS.tsv");
            //CRiskReportGeneratorExcel.GenerateExcelReport(lstoElementAnalysed, strReportFilePath.Replace(".guru", ".xlsx"));

            CRiskReportGeneratorFlatFile.GenerateReport(lstoElementAnalysed, "\t", reportsPath);
            Logger.Instance.LogEntries.Add(new LogEntry(" Report 1 of 2 generated @ " + reportsPath));

            CRiskReportGeneratorFlatFile.GenerateReport(lstoElementAnalysed, "\t", reportsPath, true);
            Logger.Instance.LogEntries.Add(new LogEntry(" Report 2 of 2 generated @ " + reportsPath));

            Logger.Instance.LogEntries.Add(new LogEntry("'Submit' completed."));

        }

        #endregion SubmitCommand

        #region DialogCommand
        public ICommand DialogCommand { get { return new RelayCommand(DialogAction); } }

        public void DialogAction(object a)
        {
            DialogTextContent = null;
            DialogArgument = a;
        }
        #endregion DialogCommand

        #region DialogArgument
        private object _dialogArgument;

        public object DialogArgument
        {
            get { return _dialogArgument; }
            set { SetProperty(ref _dialogArgument, value); }
        }
        #endregion DialogArgument

        #endregion commands

        #region methods
        public void ToggleView(object view)
        {
            if (ViewsList.Contains(view)) ViewsList.Remove(view);
            else ViewsList.Add(view);

        }
        #endregion methods
    }
}
