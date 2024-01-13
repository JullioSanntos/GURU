using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GURU.Tests.Model
{
    [TestClass]
    public class JsonTutorialTests
    {
        [TestMethod]
        public void FileInfoTest()
        {
            var sut = JsonTutorial.SavedFilesFileInfo;
            var fileFullName = Path.Combine(JsonTutorial.SavedFilesFileInfo.DirectoryName, nameof(FileInfoTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            if (fileInfo.Exists) { fileInfo.Delete(); }
            Assert.IsFalse(fileInfo.Exists); 
        }

        [TestMethod]
        public void SerializeTest()
        {
            var sut = new JsonTutorial();
            var fileFullName = Path.Combine(JsonTutorial.SavedFilesFileInfo.DirectoryName, nameof(SerializeTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();

            sut.Serialize(fileFullName);
             
            Assert.IsTrue(fileInfo.Exists);
        }

        [TestMethod]
        public void SerializeTwiceTest()
        {
            var sut = new JsonTutorial();
            var fileFullName = Path.Combine(JsonTutorial.SavedFilesFileInfo.DirectoryName, nameof(SerializeTwiceTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();

            var tutorial = new Tutorial() { ID = 1 };
            sut.Serialize(fileFullName, tutorial);
            tutorial.ID = 2;
            sut.Serialize(fileFullName, tutorial);
            Assert.IsTrue(fileInfo.Exists);

            var newTutorial = sut.Deserialize(fileFullName);

            Assert.AreEqual(newTutorial.ID, tutorial.ID);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            var sut = new JsonTutorial();
            var fileFullName = Path.Combine(JsonTutorial.SavedFilesFileInfo.DirectoryName, nameof(DeserializeTest) + ".txt");
            var fileInfo = new FileInfo(fileFullName);
            fileInfo.Delete();
            sut.Serialize(fileFullName);

            var tutorial = sut.Deserialize(fileFullName);

            Assert.IsNotNull(tutorial);
            Assert.IsNotNull(tutorial.Name);
        }
    }
}
