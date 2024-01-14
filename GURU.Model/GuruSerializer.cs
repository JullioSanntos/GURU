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

        public void Serialize<T>(string fileFullname, T fileInfos)
        {
            //if (tutorial == null) tutorial = new Tutorial { ID = 1, Name = ".Net" };

            using (StreamWriter file = File.CreateText(fileFullname)) {
                var serializingSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.All };
                var serializer = JsonSerializer.CreateDefault(serializingSettings);
                //JsonSerializer serializer = new JsonSerializer();
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
