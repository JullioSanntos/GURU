using GURU.Common;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GURU.Tests.Model
{
    [TestClass]
    public class GuruSerializerTests
    {
        [TestMethod]
        public void FileInfoTest()
        {
            var sut = GuruSerializer.SavedFilesFileInfo;
            var fileFullName = Path.Combine(GuruSerializer.SavedFilesFileInfo.DirectoryName, nameof(FileInfoTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            if (fileInfo.Exists) { fileInfo.Delete(); }
            Assert.IsFalse(fileInfo.Exists); 
        }

        [TestMethod]
        public void SerializeTest()
        {
            var sut = new GuruSerializer();
            var fileFullName = Path.Combine(GuruSerializer.SavedFilesFileInfo.DirectoryName, nameof(SerializeTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();
            var filesList = new ExtendedObservableCollection<SerilzFileInfo>();
            sut.Serialize(fileFullName, filesList);
             
            Assert.IsTrue(fileInfo.Exists);
        }

        [TestMethod]
        public void SerializeTwiceTest()
        {
            var sut = new GuruSerializer();
            var fileFullName = Path.Combine(GuruSerializer.SavedFilesFileInfo.DirectoryName, nameof(SerializeTwiceTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();

            var filesList = new ExtendedObservableCollection<SerilzFileInfo>() { };
            filesList.Add(new SerilzFileInfo(fileInfo.FullName));
            sut.Serialize(fileFullName, filesList);
            filesList.Add(new SerilzFileInfo(fileInfo.FullName));
            sut.Serialize(fileFullName, filesList);
            Assert.IsTrue(fileInfo.Exists);

            var newFileInfos = sut.Deserialize(fileFullName);

            Assert.AreEqual(newFileInfos.Count, 2);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            var sut = new GuruSerializer();
            var fileFullName = Path.Combine(GuruSerializer.SavedFilesFileInfo.DirectoryName, nameof(DeserializeTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();
            var filesList = new ExtendedObservableCollection<SerilzFileInfo>();
            filesList.Add(new SerilzFileInfo(fileInfo.FullName));
            filesList.Add(new SerilzFileInfo(fileInfo.FullName));
            sut.Serialize(fileFullName, filesList);

            var tutorial = sut.Deserialize(fileFullName);

            Assert.IsNotNull(tutorial);
            //Assert.IsNotNull(tutorial.Name);
        }
    }
}
