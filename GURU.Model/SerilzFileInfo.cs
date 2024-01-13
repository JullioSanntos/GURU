using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;

namespace GURU.Model
{
    public class SerilzFileInfo : BindableBase
    {

        #region FilePath
        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            private set
            {
                SetProperty(ref _filePath, value);
                RaisePropertyChanged(nameof(FileInfo));
            }
        }
        #endregion FilePath

        #region FileInfo
        private FileInfo _fileInfo;

        public FileInfo FileInfo
        {
            get
            {
                if (_fileInfo == null)
                {
                    _fileInfo = new FileInfo(FilePath);
                    RaisePropertyChanged(nameof(FileInfo));
                }

                return _fileInfo;
            }
        }
        #endregion FileInfo


        //#region FilesWatcher
        //private FileSystemWatcher _filesWatcher;

        //public FileSystemWatcher FilesWatcher
        //{
        //    get { return _filesWatcher; }
        //    set {
        //        if (_filesWatcher != null) {
        //            _filesWatcher.Changed -= FileInfoFolderChanged;
        //            _filesWatcher.Deleted -= FileInfoFolderChanged;
        //            _filesWatcher.Renamed -= FileInfoFolderChanged;
        //            _filesWatcher.Created -= FileInfoFolderChanged;
        //        }

        //        SetProperty(ref _filesWatcher, value);

        //        if (_filesWatcher != null) {
        //            _filesWatcher.Changed += FileInfoFolderChanged;
        //            _filesWatcher.Deleted += FileInfoFolderChanged;
        //            _filesWatcher.Renamed += FileInfoFolderChanged;
        //            _filesWatcher.Created += FileInfoFolderChanged;
        //        }
        //    }
        //}

        //private void FileInfoFolderChanged(object sender, FileSystemEventArgs e)
        //{

        //}
        //#endregion FilesWatcher


        public SerilzFileInfo(string filePath) { FilePath = filePath; }

        public bool FileExist { get { return FileInfo.Exists; } }


    }
}
