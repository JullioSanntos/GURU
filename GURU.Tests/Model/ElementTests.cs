using System;
using System.Linq;
using System.Threading;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests.Model
{
    [TestClass]
    public class ElementTests
    {
        [TestMethod]
        public void InstantiationTest()
        {
            var target = new Element(MainModel.Instance);
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void InstantiationTest5()
        {
            var target = new Element(null);
            Assert.IsNotNull(target);
            Assert.IsFalse(target.AvailableGradedConditionTypesList.Any());
            Assert.IsFalse(target.AvailableGradedFailureModeTypesList.Any());
            Assert.IsFalse(target.AvailableGradedStressTypesList.Any());
        }

        [TestMethod]
        public void InstantiationTest1()
        {
            var mainModel = MainModel.Instance;
            var target = new Element(mainModel);
            Assert.IsNotNull(mainModel.ElementTypes);
            target.ElementType = mainModel.ElementTypes.First();
            Assert.IsNotNull(target.ElementType);
            Assert.IsTrue(target.AvailableGradedConditionTypesList.Any());
            Assert.IsTrue(target.AvailableGradedFailureModeTypesList.Any());
            Assert.IsTrue(target.AvailableGradedStressTypesList.Any());
        }

        [TestMethod]
        public void InstantiationTest2()
        {
            var mainModel = MainModel.Instance;
            var target = new Element(mainModel);
            var condType = target.AvailableGradedConditionTypesList.First().GetClone(1);
            target.GradedConditionTypesList.Add(condType);
            Assert.AreEqual(1, target.GradedConditionTypesList.Count);
        }

        [TestMethod]
        public void ElementNameChangedTest()
        {
            var mainModel = MainModel.Instance;
            var target = new Element(mainModel);
            mainModel.Elements.Add(target);
            var elemNameChanged = false;
            var composedElementsChanged = false;
            target.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(target.Name)) elemNameChanged = true; };
            mainModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(MainModel.ComposedElements)) composedElementsChanged = true; };
            target.Name = "E1";
            Assert.IsTrue(elemNameChanged);
            Assert.IsTrue(composedElementsChanged);
        }

        [TestMethod]
        public void GlobalElementNameChangedTest()
        {
            var mainModel = MainModel.Instance;
            var target = new GlobalElement(mainModel);
            mainModel.GlobalElements.Add(target);
            var globalElemNameChanged = false;
            var composedElementsChanged = false;
            target.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(target.Name)) globalElemNameChanged = true; };
            mainModel.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(MainModel.ComposedElements)) composedElementsChanged = true; };
            target.Name = "_1";
            Assert.IsTrue(globalElemNameChanged);
            Assert.IsTrue(composedElementsChanged);
        }

        [TestMethod]
        public void InstantiationTest3()
        {
            var mainModel = MainModel.Instance;
            var target = new Element(mainModel);
            var failureType = target.AvailableGradedFailureModeTypesList.First();
            failureType.Grade = 1;
            target.GradedFailureModesTypesList.Add(failureType);
            Assert.AreEqual(1, target.GradedFailureModesTypesList.Count);
        }

        [TestMethod]
        public void InstantiationTest4()
        {
            var target = new Element(MainModel.Instance);
            var stressType = target.AvailableGradedStressTypesList.First();
            stressType.Grade = 1;
            target.GradedInitialStressTypesList.Add(stressType);
            Assert.AreEqual(1, target.GradedInitialStressTypesList.Count);
        }

        [TestMethod]
        public void AddGradedConditionTest()
        {
            var target = new Element(MainModel.Instance);
            var allGradedConditionsCount = target.AvailableGradedConditionTypesList.Count;
            var gradedConditionType = target.AvailableGradedConditionTypesList.First().GetClone(1);
            target.GradedConditionTypesList.Add(gradedConditionType);
            var addedGradedConditionStillAvailable = target.AvailableGradedConditionTypesList.FirstOrDefault((a) => gradedConditionType == a);
            Assert.AreEqual(addedGradedConditionStillAvailable.Grade, gradedConditionType.Grade);
        }

        [TestMethod]
        public void AddGradedConditionWithGradeZeroTest()
        {
            var target = new Element(MainModel.Instance);
            var gradedConditionType = target.AvailableGradedConditionTypesList.First().GetClone(0);
            target.GradedConditionTypesList.Add(gradedConditionType);
            Assert.IsTrue(target.GradedConditionTypesList.Any());
        }

        [TestMethod]
        public void AddGradedConditionWithInvalidZeroTest()
        {
            var target = new Element(MainModel.Instance);
            var gradedConditionType = target.AvailableGradedConditionTypesList.First().GetClone(7);
            target.GradedConditionTypesList.Add(gradedConditionType);
            Assert.IsFalse(target.GradedConditionTypesList.Any());
        }

        [TestMethod]
        public void AddGradedStressingWithGradeZeroTest()
        {
            var target = new Element(MainModel.Instance);
            var gradedStressType = target.AvailableGradedStressTypesList.First().GetClone(0);
            target.GradedInitialStressTypesList.Add(gradedStressType);
            Assert.IsTrue(target.GradedInitialStressTypesList.Any());
        }

        [TestMethod]
        public void AddGradedStressingWithInvalidGradeTest()
        {
            var target = new Element(MainModel.Instance);
            var gradedStressType = target.AvailableGradedStressTypesList.First().GetClone(-1);
            target.GradedInitialStressTypesList.Add(gradedStressType);
            Assert.IsFalse(target.GradedInitialStressTypesList.Any());
        }

        [TestMethod]
       public void RaiseEventWhenGradedConditionAddedTest()
        {
            var target = new Element(MainModel.Instance);
            var eventRaisedCtr = 0;
            target.AvailableGradedConditionTypesList.CollectionChanged += ((s, e) => { eventRaisedCtr++; });
            var gradedConditionType = target.AvailableGradedConditionTypesList.First().GetClone(1);
            target.GradedConditionTypesList.Add(gradedConditionType);
            Assert.IsTrue(eventRaisedCtr > 0);
            var addedGradedConditionStillAvailable = target.AvailableGradedConditionTypesList.Any((a) => target.GradedConditionTypesList.First() == a);
            Assert.IsTrue(addedGradedConditionStillAvailable);
        }

        [TestMethod]
        public void RemoveGradedConditionTest()
        {
            var target = new Element(MainModel.Instance);
            var allGradedConditionsCount = target.AvailableGradedConditionTypesList.Count;
            var gradedConditionType = target.AvailableGradedConditionTypesList.First().GetClone(3);
            target.GradedConditionTypesList.Add(gradedConditionType);
            target.GradedConditionTypesList.Remove(gradedConditionType);
            var addedGradedConditionStillAvailable = target.AvailableGradedConditionTypesList.FirstOrDefault((a) => target.GradedConditionTypesList.FirstOrDefault() == a);
            Assert.AreNotEqual(addedGradedConditionStillAvailable?.Grade, gradedConditionType.Grade);
        }

        [TestMethod]
        public void GradedConditionInitializationTest()
        {
            var target1 = new GradedInitialConditionType(MainModel.Instance.InitialConditionTypes.First());
            var target2 = new GradedInitialConditionType(MainModel.Instance.InitialConditionTypes.First());
            Assert.AreNotEqual(target1.GetHashCode(), target2.GetHashCode());
        }

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

        //[ClassCleanup]
        //public static void CleanUp(TestContext testContext)
        //{
        //    MainModel.Instance.Dispose();
        //}
    }
}
