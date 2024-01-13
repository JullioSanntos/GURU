using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using Newtonsoft.Json;

namespace GURU.ViewsAndViewModels
{
    public class SplashScreenViewModel : BindableBase
    {
        #region SavedFilesFileInfo
        [JsonIgnore]
        public static FileInfo SavedFilesFileInfo {
            get {
                //var fileName = Path.Combine(MainViewModel.AppDataFolderInfo.ToString(), @"SavedFiles.json");
                var fileName = Path.Combine(DependenciesDir.FullName, @"SavedFiles.json");
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists == false) fileInfo.Create();
                return fileInfo;
            }
        }
        #endregion SavedFilesFileInfo

        private FileInfo fileInfo1;

        [JsonIgnore]
        public static DirectoryInfo DependenciesDir
        {
            get {
                var binPath = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.Parent;
                var execPath = binPath.EnumerateDirectories().First();
                var depencPath = execPath.GetDirectories().First(d => d.Name == "Dependencies");
                return depencPath; 
            }
        }


        #region SavedFilesList
        private ExtendedObservableCollection<SerilzFileInfo> _savedFilesList;

        public ExtendedObservableCollection<SerilzFileInfo> SavedFilesList
        {
            get { return _savedFilesList ?? (SavedFilesList = new ExtendedObservableCollection<SerilzFileInfo>() ); }
            set
            {
                if (_savedFilesList != null) _savedFilesList.CollectionChanging -= SavedFilesList_CollectionChanging;
                if (_savedFilesList != null) _savedFilesList.CollectionChanged -= _savedFilesList_CollectionChanged;
                SetProperty(ref _savedFilesList, value);
                if (_savedFilesList != null) _savedFilesList.CollectionChanging += SavedFilesList_CollectionChanging;
                if (_savedFilesList != null) _savedFilesList.CollectionChanged += _savedFilesList_CollectionChanged;
            }

        }
        private void SavedFilesList_CollectionChanging(object sender, ItemCancelEventArgs e)
        {
            var coll = sender as ExtendedObservableCollection<SerilzFileInfo>;
            var addingItem = e.Item as SerilzFileInfo;
            // duplicate item points to the same path but is a different object (reference)
            var duplicateItem = coll?.FirstOrDefault(c => c.FilePath.Equals(addingItem?.FilePath) && c != addingItem);
            if (duplicateItem != null)
            {
                var ix = coll.IndexOf(duplicateItem);
                if (ix != 0) coll.Move(ix, 0);
                e.Cancel = true;
            }
            else
            {
                if (coll?.Count > 9) coll.RemoveItem(9);
            }
        }

        private void _savedFilesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }
        #endregion SavedFilesList

        #region SelectedFile
        [JsonIgnore]
        private SerilzFileInfo _selectedFile;

        public SerilzFileInfo SelectedFile
        {
            get { return _selectedFile; }
            set { SetProperty(ref _selectedFile, value); RaisePropertyChanged(nameof(IsFileSelected)); }
        }
        #endregion SelectedFile

        #region IsFileSelected
        public bool IsFileSelected { get { return SelectedFile != null; } }
        #endregion IsFileSelected


    }


}
