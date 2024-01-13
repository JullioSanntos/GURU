using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using GURU.Common;
using GURU.Common.Interfaces;
using GURU.Common.Log;
using GURU.Model;
using GURU.ViewsAndViewModels;
using Newtonsoft.Json;

namespace GURU
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private MainView MainView { get; } = new MainView();
        private TextWriter logWritter;
        public App()
        {
            Common.DI.Container = GetCompositionContainer();
            Common.DI.Container.ComposeParts(this);
            BindableBase.Dispatcher = this.Dispatcher;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            this.Exit += App_Exit;
            var shortDate = $"{DateTime.Now:yyyyMMdd.HH}";
            var logFileInfo = new FileInfo(MainModel.AppDataFolderInfo + $@"\{shortDate}_Guru.log");
            logWritter = logFileInfo.AppendText();
            logWritter.WriteLineAsync($@"********************** {shortDate} ******************************************************** ");
            Logger.Instance.LogEntries.CollectionChanged 
                += (s, e) => { foreach (var i in e.NewItems) {logWritter.WriteLine(i.ToString().ToCharArray()); logWritter.Flush();} };



            //GetCompositionContainer();
        }

        public CompositionContainer GetCompositionContainer()
        {
            // Combine all the registration Exports defined using Fluent syntax in each known project
            var registrations = new RegistrationBuilder();
            GURU.DI.AddRegistrations(registrations);
            Model.DI.CreationPolicyPerType.Add(typeof(MainModel), CreationPolicy.Shared);
            Model.DI.AddRegistrations(registrations);
            Common.DI.AddRegistrations(registrations);
            //registrationBuilder.ForType<MainModel>().Export<MainModel>(); // this builder overrides the same registration that exists in the MainModel, just for tests

            // Get all the registrations Exports and Imports that were declared through a .net attribute (declarative syntax)
            var catalog1 = new AssemblyCatalog(this.GetType().Assembly, registrations); // Add registrations here instead of the Model project just for tests
            var catalog2 = new AssemblyCatalog(typeof(Model.DI).Assembly);
            var catalog3 = new AssemblyCatalog(typeof(Common.DI).Assembly);
            var aggrCatalog = new AggregateCatalog();
            aggrCatalog.Catalogs.Add(catalog1);
            aggrCatalog.Catalogs.Add(catalog2);
            aggrCatalog.Catalogs.Add(catalog3);
            var compositionContainer = new CompositionContainer(aggrCatalog, CompositionOptions.DisableSilentRejection);
            return compositionContainer;

            //Common.DI.Container = new CompositionContainer(aggrCatalog, CompositionOptions.DisableSilentRejection);
            // Compose parts and test results
            //Common.DI.Container.ComposeParts(this);
        }

        ////[Import]
        //public MainModel MyMainModel { get { return Common.DI.Container.GetExportedValue<MainModel>(); } }

        //[Import(RequiredCreationPolicy = CreationPolicy.NonShared)]
        //public MainModel MyMainModelNonShared { get; set; }


        public bool SavedFilesListGotDeserialized { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            SavedFilesListGotDeserialized = TryDeserializeSavedFilesList();

        }


        private void App_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (SavedFilesListGotDeserialized) TrySerializeSavedFilesList();
            }
            catch (Exception exception)
            {
                Logger.Instance.LogEntries.Add(new LogEntry(exception));
            }
            logWritter.Flush();
            logWritter.Close();
        }


        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogEntries.Add(new LogEntry(e.Exception));

            logWritter.Flush();

            var dialogFactory = ServicesLocator.GetService(ServicesLocator.DialogServicesKey);
            if (dialogFactory(null) is DialogViewModel dvm)
            {
                dvm.Message = e.Exception.Message + Environment.NewLine + "Danger, Will Robinson, unexpected exception!!!";
                dvm.Title = "Application dialog";
                dvm.CommandsList = new List<DialogSelection>() {DialogSelection.OK};
                dvm.ShowDialog();
                e.Handled = true;
            }

            logWritter.WriteLine(e.Exception);
            Current.Shutdown(e.Exception.HResult);
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            //base.OnStartup(e);

            //if (!Utils.IsInDesignTool)
            //{
            //    var asmCatalog = new AssemblyCatalog(typeof(App).Assembly);
            //    var catalogs = new AggregateCatalog(asmCatalog);
            //    var container = new CompositionContainer(catalogs);
            //    CompositionHost.Initialize(container);
            //    container.Compose(new CompositionBatch());

            //    MainView.ShowDialog();
            //}

            //AssemblyCatalog guiCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            //AssemblyCatalog modelcatalog = new AssemblyCatalog(typeof(MainModel).Assembly);
            //AggregateCatalog allCatalogs = new AggregateCatalog(guiCatalog, modelcatalog);
            //CompositionContainer container = new CompositionContainer(allCatalogs);
            
            //container.SatisfyImportsOnce(new ComposablePart());
            ////container.Compose(new CompositionBatch());

        }


        public bool TryDeserializeSavedFilesList()
        {
            try
            {
                var splashScreenViewModel = this.Resources[nameof(SplashScreenViewModel)] as SplashScreenViewModel;

                //if SavedFilesList is empty add a working GURU file as a demo
                if (SplashScreenViewModel.SavedFilesFileInfo.Exists == false)
                {
 
                    var guruFile = SplashScreenViewModel.DependenciesDir.GetFiles().First(file => file.Name.EndsWith("guru"));
                    if (guruFile.Exists)
                    {
                        var srlzdGuruFile = new SerilzFileInfo(guruFile.FullName);
                        splashScreenViewModel.SavedFilesList.Add(srlzdGuruFile);
                    }
                }
                TrySerializeSavedFilesList();
                var savedFilesPath = SplashScreenViewModel.SavedFilesFileInfo.ToString();
                var savedFilesListContent = File.ReadAllText(savedFilesPath);
                if (splashScreenViewModel != null)
                    splashScreenViewModel.SavedFilesList =
                        JsonConvert.DeserializeObject<ExtendedObservableCollection<SerilzFileInfo>>(savedFilesListContent,
                            new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            }
            catch (Exception e)
            {
                Logger.Instance.LogEntries.Add(new LogEntry(e));
                return false;
            }
            

            return true;
        }

        public bool TrySerializeSavedFilesList()
        {
            try
            {
                var splashScreenViewModel = this.Resources[nameof(SplashScreenViewModel)] as SplashScreenViewModel;
                if (splashScreenViewModel == null) throw new Exception("'SplashScreenViewModel' resource not found");

                var savedFilesPath = SplashScreenViewModel.SavedFilesFileInfo.ToString();
                var serializingSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.All };
                var srldObj = JsonConvert.SerializeObject(splashScreenViewModel.SavedFilesList, Formatting.Indented, settings: serializingSettings);
                File.WriteAllText(savedFilesPath, srldObj);
            }
            catch (Exception e)
            {
                Logger.Instance.LogEntries.Add(new LogEntry(e));
                return false;
            }

            return true;
        }

    }
}
