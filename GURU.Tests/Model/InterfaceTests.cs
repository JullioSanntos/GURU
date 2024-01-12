using System;
using System.Linq;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests.Model
{
    [TestClass]
    public class InterfaceTests
    {
        //[TestMethod]
        //public void Element1ElementsTest()
        //{
        //    var target = new Interface();
        //    MainModel.MainModelInstance.Elements.Add(new Element() {Name = "E1"});
        //    MainModel.MainModelInstance.Elements.Add(new Element() {Name = "E2"});
        //    MainModel.MainModelInstance.Elements.Add(new Element() {Name = "E3" });
        //    Assert.AreEqual(MainModel.MainModelInstance.Elements.Count, target.Element1Elements.Count);
        //    target.Element1 = MainModel.MainModelInstance.Elements.First();
        //    Assert.AreEqual(MainModel.MainModelInstance.Elements.Count - 1, target.Element1Elements.Count);
        //    Assert.IsFalse(target.Element1Elements.Contains(target.Element1));
        //}

        //[TestMethod]
        //public void Element2ElementsTest()
        //{
        //    var target = new Interface();
        //    MainModel.MainModelInstance.Elements.Add(new Element() { Name = "E1" });
        //    MainModel.MainModelInstance.Elements.Add(new Element() { Name = "E2" });
        //    MainModel.MainModelInstance.Elements.Add(new Element() { Name = "E3" });
        //    Assert.AreEqual(MainModel.MainModelInstance.Elements.Count, target.Element1Elements.Count);
        //    target.Element2 = MainModel.MainModelInstance.Elements.First();
        //    Assert.AreEqual(MainModel.MainModelInstance.Elements.Count - 1, target.Element2Elements.Count);
        //    Assert.IsFalse(target.Element2Elements.Contains(target.Element2));
        //}

        [TestMethod]
        public void NameTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = new Interface();
                Assert.IsNull(target.Name);
                var name = "My Name";
                target.Name = name;
                Assert.AreEqual(name, target.Name);
                mainModel.Elements.Add(new Element(mainModel) { Name = "E1" });
                mainModel.Elements.Add(new Element(mainModel) { Name = "E2" });
                mainModel.Elements.Add(new Element(mainModel) { Name = "E3" });
                target.Element1 = mainModel.Elements.First();
                Assert.IsTrue(target.Name.Contains(target.Element1.Name));
                target.Element2 = mainModel.Elements.Skip(1).First();
                Assert.IsTrue(target.Name.Contains(target.Element1.Name) && target.Name.Contains(target.Element2.Name));

            }
        }
    }
}
