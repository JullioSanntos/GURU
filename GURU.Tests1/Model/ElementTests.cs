using System;
using System.Linq;
using System.Threading;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests1.Model
{
    [TestClass]
    public class ElementTests
    {
        [TestMethod]
        public void InstantiationTest()
        {
            var target = new Element();
            Assert.IsNotNull(target);
        }

        //[TestMethod]
        //public void AddGradedConditionTest()
        //{
        //    var target = new Element();
        //    var allGradedConditionsCount = target.AvailableGradedConditionTypesList.Count;
        //    var gradedConditionType = target.AvailableGradedConditionTypesList.First();
        //    target.GradedConditionTypesList.Add(gradedConditionType);
        //    var expected = allGradedConditionsCount - 1;
        //    Assert.AreEqual(expected, allGradedConditionsCount - 1);
        //    Assert.IsFalse(target.AvailableGradedConditionTypesList.Any((a) => Equals(gradedConditionType, a)));
        //}

        //[TestMethod]
        //public void RaiseEvenWhenGradedConditionAddedTest()
        //{
        //    var target = new Element();
        //    var eventRaisedCtr = 0;
        //    target.AvailableGradedConditionTypesList.CollectionChanged += ((s,e) => {if (e.OldItems?.Count == 1) eventRaisedCtr++;});
        //    var gradedConditionType = target.AvailableGradedConditionTypesList.First();
        //    target.GradedConditionTypesList.Add(gradedConditionType);
        //    Assert.AreEqual(1, eventRaisedCtr);
        //    Assert.IsFalse(target.AvailableGradedConditionTypesList.Any((a) => Equals(gradedConditionType, a)));
        //}

        //[TestMethod]
        //public void RaiseEvenWhenGradedConditionRemovedTest()
        //{
        //    var target = new Element();
        //    var eventRaisedCtr = 0;
        //    target.AvailableGradedConditionTypesList.CollectionChanged += ((s, e) => { if (e.NewItems?.Count == 1) eventRaisedCtr++; });
        //    var gradedConditionType = target.AvailableGradedConditionTypesList.First();
        //    target.GradedConditionTypesList.Add(gradedConditionType);
        //    target.GradedConditionTypesList.Remove(gradedConditionType);
        //    Assert.AreEqual(1, eventRaisedCtr);
        //    Assert.IsTrue(target.AvailableGradedConditionTypesList.Any((a) => Equals(gradedConditionType, a)));
        //}

    }
}
