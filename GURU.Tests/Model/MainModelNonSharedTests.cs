using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Threading.Tasks;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;

namespace GURU.Tests.Model
{
    /// <summary>
    /// Summary description for MainModelSharedTests
    /// </summary>
    [TestClass]
    public class MainModelNonSharedTests
    {

        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void ComposeModel(TestContext testContext)
        {
            var registrations = new RegistrationBuilder();
            GURU.Model.DI.CreationPolicyPerType.Add(typeof(MainModel), CreationPolicy.NonShared);
            GURU.Model.DI.AddRegistrations(registrations);
            GURU.Common.DI.AddRegistrations(registrations);
            //registrationBuilder.ForType<MainModel>().Export<MainModel>(); // this builder overrides the same registration that exists in the MainModel, just for tests

            // Get all the registrations Exports and Imports that were declared through a .net attribute (declarative syntax)
            var catalog2 = new AssemblyCatalog(typeof(GURU.Model.DI).Assembly);
            var catalog3 = new AssemblyCatalog(typeof(Common.DI).Assembly);
            var aggrCatalog = new AggregateCatalog();
            aggrCatalog.Catalogs.Add(catalog2);
            aggrCatalog.Catalogs.Add(catalog3);
            Common.DI.Container = new CompositionContainer(aggrCatalog, CompositionOptions.DisableSilentRejection);

        }



        [TestMethod]
        public void CompositionINitializationTest()
        {
            int hashCode1;
            using (var mainModel1 = MainModel.Instance)
            {
                Assert.IsNotNull(mainModel1);
                hashCode1 = mainModel1.GetHashCode();
            }

            int hashCode2;
            using (var mainModel2 = MainModel.Instance)
            {
                Assert.IsNotNull(mainModel2);
                hashCode2 = mainModel2.GetHashCode();

            }

            Assert.AreNotEqual(hashCode1, hashCode2);
        }


        [TestMethod]
        public void AllPossibleInterfacesTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.AllPossibleInterfaces;
                mainModel.Elements.Add(new Element(mainModel));
                mainModel.Elements.Add(new Element(mainModel));
                mainModel.Elements.Add(new Element(mainModel));
                Assert.AreEqual(6, target.Count());
            }
        }

        [TestMethod]
        public void AllAvailableInterfacesCalculationTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.AllAvailableInterfaces;
                mainModel.Elements.Add(new Element(mainModel));
                mainModel.Elements.Add(new Element(mainModel));
                mainModel.Elements.Add(new Element(mainModel));
                Assert.AreEqual(6, mainModel.AllAvailableInterfaces.Count());
                Assert.AreEqual(6, target.Count());
            }
        }

        [TestMethod]
        public void ElementGradeConditionTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                mainModel.Elements.Add(new Element(mainModel));
                var elem1 = mainModel.Elements.First().GetClone(mainModel);
                Assert.IsFalse(elem1.GradedConditionTypesList.Any());
                elem1.GradedConditionTypesList.Add(elem1.AvailableGradedConditionTypesList.First().GetClone(1));
                Assert.IsTrue(elem1.GradedConditionTypesList.Any());
            }
        }

        [TestMethod]
        public void GlobalElementsInitializationTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);
                target.Add(new GlobalElement(mainModel));
                Assert.AreEqual(1, target.Count);
                Assert.IsFalse(mainModel.ComposedElements.Any());
            }
        }

        [TestMethod]
        public void ComposedElementsInitializationTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);
                mainModel.Elements.Add(new Element(mainModel));
                Assert.IsNotNull(mainModel.ComposedElements);
            }
        }

        [TestMethod]
        public void GlobalElementsApplyOrderTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = new GlobalElement(mainModel);
                Assert.AreEqual(10, target.Priority);
                var cond = target.AvailableGradedConditionTypesList.First().GetClone(1);
                target.GradedConditionTypesList.Add(cond);
                Assert.AreEqual(10, target.Priority);
                target.ElementType = mainModel.ElementTypes.First().GetClone();
                Assert.AreEqual(20, target.Priority);
                target.ElementType = null;
                target.Name = "NAME";
                Assert.AreEqual(30, target.Priority);
                target.ElementType = mainModel.ElementTypes.First().GetClone();
                Assert.AreEqual(40, target.Priority);
            }
        }

        [TestMethod]
        public void GlobalElements1E1GTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);
                target.Add(new GlobalElement(mainModel));

                mainModel.Elements.Add(new Element(mainModel));

                Assert.AreEqual(1, target.Count);
                Assert.IsFalse(mainModel.ComposedElements.First().GradedInitialStressTypesList.Any());
                Assert.IsFalse(mainModel.ComposedElements.First().GradedConditionTypesList.Any());
            }
        }

        [TestMethod]
        public void ComposedElements1E0GTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);
                target.Add(new GlobalElement(mainModel));

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                var stress1 = elem1.AvailableGradedStressTypesList.First().GetClone(2);
                elem1.GradedInitialStressTypesList.Add(stress1);
                mainModel.Elements.Add(elem1);

                var compdElem = mainModel.ComposedElements.First();

                Assert.AreEqual(cond1.ToString(), compdElem.GradedConditionTypesList.First().ToString());
                Assert.AreEqual(cond1.Grade, compdElem.GradedConditionTypesList.First().Grade);
                Assert.AreEqual(stress1.ToString(), compdElem.GradedInitialStressTypesList.First().ToString());
                Assert.AreEqual(stress1.Grade, compdElem.GradedInitialStressTypesList.First().Grade);
            }
        }

        [TestMethod]
        public void GlobalElements1E1GTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = gElem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                Assert.AreEqual(1, mainModel.ComposedElements.Count);
                Assert.AreEqual(2, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
            }
        }

        [TestMethod]
        public void GlobalElements1E2GTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gElem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(4);
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(1, mainModel.ComposedElements.Count);
                Assert.AreEqual(2, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
            }
        }

        [TestMethod]
        public void GlobalElements1E2GTStressesTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var stress1 = elem1.AvailableGradedStressTypesList.First().GetClone(1);
                elem1.GradedInitialStressTypesList.Add(stress1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gstress1 = gElem1.AvailableGradedStressTypesList.First().GetClone(5);
                gElem1.GradedInitialStressTypesList.Add(gstress1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gStress2 = gElem1.AvailableGradedStressTypesList.Skip(1).First().GetClone(4);
                gElem2.GradedInitialStressTypesList.Add(gStress2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(1, mainModel.ComposedElements.Count);
                Assert.AreEqual(2, mainModel.ComposedElements.First().GradedInitialStressTypesList.Count);
            }
        }

        [TestMethod]
        public void GlobalElements1E2GMatchingByElementTypeTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel) {ElementType = mainModel.ElementTypes.First().GetClone()};
                mainModel.Elements.Add(elem1);

                // this Global setting is applied because of matching ElementTypes
                var gElem1 = new GlobalElement(mainModel) { ElementType = mainModel.ElementTypes.First().GetClone() };
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                // this more generic Global setting is not applied because the previous one was applied first
                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gElem2.AvailableGradedConditionTypesList.First().GetClone(4);
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(1, mainModel.ComposedElements.Count);
                Assert.AreEqual(1, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
                Assert.AreEqual(5, mainModel.ComposedElements.First().GradedConditionTypesList.First().Grade);
            }
        }

        [TestMethod]
        public void GlobalElements2E2GMatchingByNameTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel) {  Name = "Elem1" };
                mainModel.Elements.Add(elem1);

                var elem2 = new Element(mainModel) { Name = "OtherElem1" };
                mainModel.Elements.Add(elem2);

                // this Global setting is applied because of matching Names
                var gElem1 = new GlobalElement(mainModel) { Name = "Other.*" };
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                // this more generic Global setting is only applied to Element named "Elem1"
                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gElem2.AvailableGradedConditionTypesList.First().GetClone(4);
                gCond2.Grade = 4;
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(2, mainModel.ComposedElements.Count);
                var otherElem = mainModel.ComposedElements.First(ce => ce.Name.StartsWith("Other"));
                Assert.AreEqual(1, otherElem.GradedConditionTypesList.Count);
                Assert.AreEqual(5, otherElem.GradedConditionTypesList.First().Grade);
                var elem = mainModel.ComposedElements.First(ce => ce.Name.StartsWith("Elem1"));
                Assert.AreEqual(1, elem.GradedConditionTypesList.Count);
                Assert.AreEqual(4, elem.GradedConditionTypesList.First().Grade);
            }
        }

        [TestMethod]
        public void GlobalElements2E2GMatchingByNameAndElementTypeTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel) {
                    Name = "Elem1"
                    , ElementType = mainModel.ElementTypes.First().GetClone()
                };
                mainModel.Elements.Add(elem1);

                var elem2 = new Element(mainModel) {
                    Name = "OtherElem1"
                    , ElementType = mainModel.ElementTypes.First().GetClone()

                };
                mainModel.Elements.Add(elem2);

                // this Global setting is applied because of matching Names
                var gElem1 = new GlobalElement(mainModel) {
                    Name = "Other.*"
                    , ElementType = mainModel.ElementTypes.First().GetClone()
                };
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                // this more generic Global setting is only applied to Element named "Elem1"
                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gElem2.AvailableGradedConditionTypesList.First().GetClone(4);
                gCond2.Grade = 4;
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(2, mainModel.ComposedElements.Count);
                var otherElem = mainModel.ComposedElements.First(ce => ce.Name.StartsWith("Other"));
                Assert.AreEqual(1, otherElem.GradedConditionTypesList.Count);
                Assert.AreEqual(5, otherElem.GradedConditionTypesList.First().Grade);
                var elem = mainModel.ComposedElements.First(ce => ce.Name.StartsWith("Elem1"));
                Assert.AreEqual(1, elem.GradedConditionTypesList.Count);
                Assert.AreEqual(4, elem.GradedConditionTypesList.First().Grade);
            }
        }

        [TestMethod]
        public void GlobalElements1E1GWithZeroGradeTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);
                Assert.AreEqual(1, mainModel.Elements.First().GradedConditionTypesList.First().Grade);
                Assert.IsTrue(mainModel.ComposedElements.First().GradedConditionTypesList.Any());

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(0);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                Assert.IsFalse(mainModel.ComposedElements.First().GradedConditionTypesList.Any());

            }
        }

        [TestMethod]
        public void GlobalElements1EWithZeroGrade1GTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(0);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                Assert.IsFalse(mainModel.ComposedElements.First().GradedConditionTypesList.Any());

            }
        }

        [TestMethod]
        public void GlobalElements1E1GWithZeroGradeTest()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(0);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                Assert.IsFalse(mainModel.ComposedElements.First().GradedConditionTypesList.Any());
            }
        }

        [TestMethod]
        public void GlobalElements1E2GWithZeroGradeAndElementTypeTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(0);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                //higher prioirty
                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gCond1.GetClone(2);
                Assert.AreEqual(gCond1.Name, gCond2.Name);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.IsFalse( mainModel.ComposedElements.First().GradedConditionTypesList.Any());
            }
        }

        [TestMethod]
        public void GlobalElements1E2GWithZeroGradeAndElementTypeTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(2);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gCond1.GetClone(0);
                Assert.AreEqual(gCond1.Name, gCond2.Name);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(1, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
                Assert.AreEqual(gCond1.Grade, mainModel.ComposedElements.First().GradedConditionTypesList.First().Grade);
            }
        }

        [TestMethod]
        public void GlobalElements1E2GWithZeroGradeAndElementTypeTest3()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(0);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                // higher priority
                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gCond1.GetClone(1);
                Assert.AreEqual(gCond1.Name, gCond2.Name);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.IsFalse(mainModel.ComposedElements.First().GradedConditionTypesList.Any());
            }
        }
        [TestMethod]
        public void GlobalElements1EWith3Conditions2GWithZeroGradeAndElementTypeTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(1);
                var cond2 = elem1.AvailableGradedConditionTypesList.Skip(2).First().GetClone(1);
                var cond3 = elem1.AvailableGradedConditionTypesList.Skip(3).First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                elem1.GradedConditionTypesList.Add(cond2);
                elem1.GradedConditionTypesList.Add(cond3);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(5);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gCond1.GetClone(0);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(3, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
                var compdElemCond = mainModel.ComposedElements.First().GradedConditionTypesList.FirstOrDefault(gc => gc.Name == gCond2.Name);
                Assert.IsNotNull(compdElemCond);
                Assert.AreEqual(gCond1.Grade, compdElemCond.Grade);
            }
        }

        [TestMethod]
        public void GlobalElements1EWith3Conditions2GWithZeroGradeAndElementTypeTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var target = mainModel.GlobalElements;
                Assert.IsNotNull(target);

                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(1);
                var cond2 = elem1.AvailableGradedConditionTypesList.Skip(2).First().GetClone(1);
                var cond3 = elem1.AvailableGradedConditionTypesList.Skip(3).First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                elem1.GradedConditionTypesList.Add(cond2);
                elem1.GradedConditionTypesList.Add(cond3);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(0);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = gCond1.GetClone(5);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.AreEqual(mainModel.Elements.First().GradedConditionTypesList.Count - 1, mainModel.ComposedElements.First().GradedConditionTypesList.Count);
                var compdElemCond = mainModel.ComposedElements.First().GradedConditionTypesList.FirstOrDefault(gc => gc.Name == gCond2.Name);
                Assert.IsNull(compdElemCond);
            }
        }

        [TestMethod]
        public void ClearZeroGradeElementsWithConditionsTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);
                var compElems = mainModel.ComposedElements.ToList();

                //Assert.IsFalse(compElems.First().GradedConditionTypesList.Any());
                Assert.IsTrue(compElems.First().HasGradedTypes);
            }
        }

        [TestMethod]
        public void ClearZeroGradeElementsWithStressesTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var stress1 = elem1.AvailableGradedStressTypesList.Skip(1).First().GetClone(0);
                elem1.GradedInitialStressTypesList.Add(stress1);
                mainModel.Elements.Add(elem1);
                var compElems = mainModel.ComposedElements.ToList();

                Assert.IsFalse(compElems.First().HasGradedTypes);
            }
        }

        [TestMethod]
        public void ClearZeroGradeElementsWithConditionsAndStressesTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(0);
                elem1.GradedConditionTypesList.Add(cond1);
                var stress1 = elem1.AvailableGradedStressTypesList.Skip(1).First().GetClone(0);
                elem1.GradedInitialStressTypesList.Add(stress1);
                mainModel.Elements.Add(elem1);

                Assert.IsFalse(mainModel.ComposedElements.First().HasGradedTypes);
            }
        }

        [TestMethod]
        public void ClearZeroGradeElementsWithConditionsTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(0);
                elem1.GradedConditionTypesList.Add(cond1);
                var cond2 = elem1.AvailableGradedConditionTypesList.Skip(2).First().GetClone(1);
                elem1.GradedConditionTypesList.Add(cond2);
                mainModel.Elements.Add(elem1);

                var compElem = mainModel.ComposedElements.ToList();
                Assert.AreEqual(1, compElem.Count);
                Assert.AreEqual(1, compElem.First().GradedConditionTypesList.Count);
            }
        }

        [TestMethod]
        public void ClearZeroGradeElementsWithStressesTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var stress1 = elem1.AvailableGradedStressTypesList.Skip(1).First().GetClone(0);
                elem1.GradedInitialStressTypesList.Add(stress1);
                mainModel.Elements.Add(elem1);

                Assert.IsFalse(mainModel.ComposedElements.First().HasGradedTypes);
            }
        }


        [TestMethod]
        public void GlobalElements1EWith1ZeroGrade1GTest1()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(0);
                elem1.GradedConditionTypesList.Add(cond1);
                mainModel.Elements.Add(elem1);

                 var gElem2 = new GlobalElement(mainModel);
                var gCond2 = cond1.GetClone(5);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.IsFalse(mainModel.ComposedElements.First().HasGradedTypes);
            }
        }

        [TestMethod]
        public void GlobalElements1EWith1ZeroGrade2GTest2()
        {
            using (var mainModel = MainModel.Instance)
            {
                var elem1 = new Element(mainModel);
                elem1.ElementType = mainModel.ElementTypes.First();
                var cond1 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(0);
                elem1.GradedConditionTypesList.Add(cond1);
                //var cond2 = elem1.AvailableGradedConditionTypesList.Skip(2).First().GetClone(1);
                //elem1.GradedConditionTypesList.Add(cond2);
                //var cond3 = elem1.AvailableGradedConditionTypesList.Skip(3).First().GetClone(1);
                //elem1.GradedConditionTypesList.Add(cond3);
                mainModel.Elements.Add(elem1);

                var gElem1 = new GlobalElement(mainModel);
                var gCond1 = cond1.GetClone(4);
                gElem1.GradedConditionTypesList.Add(gCond1);
                mainModel.GlobalElements.Add(gElem1);

                var gElem2 = new GlobalElement(mainModel);
                var gCond2 = cond1.GetClone(5);
                gElem2.ElementType = mainModel.ElementTypes.First();
                gElem2.GradedConditionTypesList.Add(gCond2);
                mainModel.GlobalElements.Add(gElem2);

                Assert.IsFalse(mainModel.ComposedElements.First().HasGradedTypes);
            }
        }

        //[TestMethod]
        //public void GlobalElements1EtWithLogicalDelete2GTest()
        //{
        //    using (var mainModel = MainModel.Instance)
        //    {
        //        var target = mainModel.GlobalElements;
        //        Assert.IsNotNull(target);

        //        var elem1 = new Element(mainModel);
        //        var cond1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
        //        cond1.Grade = 0;
        //        elem1.GradedConditionTypesList.Add(cond1);
        //        mainModel.Elements.Add(elem1);
        //        Assert.AreEqual(0, mainModel.Elements.First().GradedConditionTypesList.First().Grade);

        //        var gElem1 = new GlobalElement(mainModel);
        //        var gCond1 = gElem1.AvailableGradedConditionTypesList.First().GetClone(5);
        //        gCond1.Grade = 1;
        //        gElem1.GradedConditionTypesList.Add(gCond1);
        //        mainModel.GlobalElements.Add(gElem1);

        //        Assert.AreEqual(0, mainModel.ComposedElements.First().GradedConditionTypesList.First().Grade);

        //    }
        //}

        [TestMethod]
        public void GetSystemHierarchyTest_2E1I()
        {

            using (var target = MainModel.Instance)
            {
                Assert.IsTrue(target.ElementTypes.Any());

                var elem1 = new Element(target) { Name = "E1", ElementType = target.ElementTypes.First() };
                var elem2 = new Element(target) { Name = "E2", ElementType = target.ElementTypes.Skip(1).First() };
                target.Elements.Add(elem1);
                target.Elements.Add(elem2);
                Assert.IsFalse(target.IsValid);
                var interf1 = new Interface(target) { Element1 = elem1, Element2 = elem2 };
                target.Interfaces.Add(interf1);
                Assert.IsNotNull(interf1.Name);
                Assert.IsTrue(target.IsValid);

                var configuration = target.MonsterMatrixReader.Configuration;
                const string rootNode = "TOP";
                var systemHierarchy = target.GetSystemHierarchy(rootNode, configuration);
                Assert.IsNotNull(systemHierarchy);
                Assert.AreEqual(1 + target.Elements.Count + target.Interfaces.Count, systemHierarchy.Nodes_Yax.Count);
                var elems = systemHierarchy.GetAllChildElementBase(rootNode);
                var interf = systemHierarchy.GetElementJunctionByName(interf1.Name);
                Assert.AreEqual(1 + target.Elements.Count, elems.Count);
                Assert.AreEqual(interf1.Name, interf.Name);
                Assert.AreEqual(elem1.Name, elems.First(e => e.Name == elem1.Name).Name);
            }

        }

        //TODO: fix this [TestMethod]
        public void GetSystemHierarchyTest_2E1IWithConditions()
        {
            using (var target = MainModel.Instance)
            {
                Assert.IsTrue(target.ElementTypes.Any());

                //var elem1 = new Element() { Name = "E1"};
                //var elem2 = new Element() { Name = "E2"};
                target.Elements.Add(new Element(target) { Name = "E1" });
                target.Elements.Add(new Element(target) { Name = "E2" });
                var elem1 = target.Elements.First();
                var elem2 = target.Elements.Skip(1).First();
                elem1.ElementType = target.ElementTypes.First();
                elem2.ElementType = target.ElementTypes.Skip(1).First();

                var condType1 = elem1.AvailableGradedConditionTypesList.First().GetClone(1);
                condType1.Grade = 1;
                elem1.GradedConditionTypesList.Add(condType1);

                var condType2 = elem1.AvailableGradedConditionTypesList.Skip(1).First().GetClone(1);
                condType1.Grade = 1;
                elem2.GradedConditionTypesList.Add(condType2);

                Assert.IsFalse(target.IsValid);
                var interf1 = new Interface(target) { Element1 = elem1, Element2 = elem2 };
                target.Interfaces.Add(interf1);
                Assert.IsNotNull(interf1.Name);
                Assert.IsTrue(target.IsValid);

                var configuration = target.MonsterMatrixReader.Configuration;
                const string rootNode = "TOP";
                var systemHierarchy = target.GetSystemHierarchy(rootNode, configuration);
                Assert.IsNotNull(systemHierarchy);
                Assert.AreEqual(1 + target.Elements.Count + target.Interfaces.Count, systemHierarchy.Nodes_Yax.Count);
                var elems = systemHierarchy.GetAllChildElementBase(rootNode);
                var cond1 = elems.First().ConditiontypeInitialWithMagnitudeList.First();
                Assert.AreEqual(condType1.Name, cond1.Name);
                var cond2 = elems.Skip(1).First().ConditiontypeInitialWithMagnitudeList.First();
                Assert.AreEqual(condType2.Name, cond2.Name);
            }
        }

        //[ClassCleanup]
        //public void CleanUp()
        //{
        //    MainModel.Instance.Dispose();
        //}
    }
}
