using GURU.Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GURU.Model
{
    [Serializable]
    public class Tutorial
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class GuruSerializer
    {

        #region SavedFilesFileInfo
        [JsonIgnore]
        public static FileInfo SavedFilesFileInfo
        {
            get
            {
                var fileName = Path.Combine(DependenciesDir.FullName, @"SavedFiles.json");
                var fileInfo = new FileInfo(fileName);
                return fileInfo;
            }
        }
        #endregion SavedFilesFileInfo

        private FileInfo fileInfo1;

        [JsonIgnore]
        public static DirectoryInfo DependenciesDir
        {
            get
            {
                var binPath = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.Parent;
                var execPath = binPath.EnumerateDirectories().First();
                var depencPath = execPath.GetDirectories().First(d => d.Name == "Dependencies");
                return depencPath;
            }
        }


        //IFormatter formatter = new BinaryFormatter();

        public void Serialize(string fileFullname, ExtendedObservableCollection<SerilzFileInfo> fileInfos)
        {
            //if (tutorial == null) tutorial = new Tutorial { ID = 1, Name = ".Net" };

            using (StreamWriter file = File.CreateText(fileFullname)) {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, fileInfos);
            }

        }

        public T Deserialize<T>(string fileFullname)
        {

            // deserialize JSON directly from a file
            T fileInfos;
            using (StreamReader file = File.OpenText(fileFullname))
            {
                JsonSerializer serializer = new JsonSerializer();
                fileInfos = (T)serializer.Deserialize(file, typeof(T));
            }

            return fileInfos;
        }
    }


}
