using System;
using System.Linq;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests1.Model
{
    [TestClass]
    public class MainModelTests
    {
        [TestMethod]
        public void AllPossibleInterfacesTest()
        {
            var target = MainModel.Instance.AllPossibleInterfaces;
            MainModel.Instance.Elements.Add(new Element());
            MainModel.Instance.Elements.Add(new Element());
            MainModel.Instance.Elements.Add(new Element());
            Assert.AreEqual(4, target.Count());
        }
        [TestMethod]
        public void AllPossibleInterfacesCalculationTest()
        {
            var target = MainModel.Instance.AllAvailableInterfaces;
            MainModel.Instance.Elements.Add(new Element());
            MainModel.Instance.Elements.Add(new Element());
            MainModel.Instance.Elements.Add(new Element());
            Assert.AreEqual(4, target.Count());
        }
    }
}
