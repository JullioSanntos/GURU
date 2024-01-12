using System;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests.Model
{
    [TestClass]
    public class GlobalElementTests
    {
        [TestMethod]
        public void InitializationTest1()
        {
            var target = new GlobalElement();
            Assert.IsNotNull(target);
        }
    }
}
