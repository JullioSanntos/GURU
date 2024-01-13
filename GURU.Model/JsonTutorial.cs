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

    public class JsonTutorial
    {

        #region SavedFilesFileInfo
        [JsonIgnore]
        public static FileInfo SavedFilesFileInfo
        {
            get
            {
                //var fileName = Path.Combine(MainViewModel.AppDataFolderInfo.ToString(), @"SavedFiles.json");
                var fileName = Path.Combine(DependenciesDir.FullName, @"SavedFiles.json");
                var fileInfo = new FileInfo(fileName);
                //if (fileInfo.Exists == false) fileInfo.Create();
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

        public void Serialize(string fileFullname, Tutorial tutorial = null)
        {
            if (tutorial == null) tutorial = new Tutorial { ID = 1, Name = ".Net" };
            //using (FileStream stream = new FileStream("Example.txt", FileMode.Create, FileAccess.Write))
            //{
            //formatter.Serialize(stream, tutorial);
            //}
            // serialize JSON directly to a file

            //using (StreamWriter file = File.CreateText(@"Tutorial.json"))  {
            using (StreamWriter file = File.CreateText(fileFullname)) {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, tutorial);
            }

        }

        public Tutorial Deserialize(string fileFullname)
        {

            // deserialize JSON directly from a file
            Tutorial tutorial;
            using (StreamReader file = File.OpenText(fileFullname))
            {
                JsonSerializer serializer = new JsonSerializer();
                tutorial = (Tutorial)serializer.Deserialize(file, typeof(Tutorial));
            }

            return tutorial;

            //// Deserialization
            //Tutorial deserializedTutorial;
            //using (FileStream stream = new FileStream("Example.txt", FileMode.Open, FileAccess.Read))
            //{
            //    deserializedTutorial = (Tutorial)formatter.Deserialize(stream);
            //}
        }
    }


}
