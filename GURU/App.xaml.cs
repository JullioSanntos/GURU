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

        }

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

        public bool TryDeserializeSavedFilesList()
        {
            try
            {
                var splashScreenViewModel = this.Resources[nameof(SplashScreenViewModel)] as SplashScreenViewModel;

                var serializer = new GuruSerializer();
                if (GuruSerializer.SavedFilesFileInfo.Exists) {
                    splashScreenViewModel.SavedFilesList = serializer.Deserialize<ExtendedObservableCollection<SerilzFileInfo>>
                         (GuruSerializer.SavedFilesFileInfo.FullName);
                }


                if (splashScreenViewModel.SavedFilesList.Any() == false)
                {
                    var guruFile = GuruSerializer.DependenciesDir.GetFiles().FirstOrDefault(gf => gf.Name.EndsWith(".guru"));
                    if (guruFile.Exists) {
                        splashScreenViewModel.SavedFilesList.Add(new SerilzFileInfo(guruFile.FullName));
                    }
                }

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

                var serializer = new GuruSerializer();
                serializer.Serialize(GuruSerializer.SavedFilesFileInfo.FullName, splashScreenViewModel.SavedFilesList);
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
