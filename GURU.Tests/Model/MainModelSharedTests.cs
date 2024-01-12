using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using GURU.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests.Model
{

    [TestClass]
    public class MainModelSharedTests
    {

        [ClassInitialize()]
        public static void MyTestInitialize(TestContext testContext)
        {
            var registrationBuilder = new RegistrationBuilder();
            GURU.Model.DI.AddRegistrations(registrationBuilder);
            Common.DI.AddRegistrations(registrationBuilder);

            var catalog1 = new AssemblyCatalog(typeof(MainModel).Assembly, registrationBuilder);
            var catalog2 = new AssemblyCatalog(typeof(GURU.Common.DI).Assembly);
            //var catalog3 = new AssemblyCatalog(typeof(InitializeTests).Assembly);
            var aggrCatalog = new AggregateCatalog();
            aggrCatalog.Catalogs.Add(catalog1);
            aggrCatalog.Catalogs.Add(catalog2);
            GURU.Common.DI.Container = new CompositionContainer(aggrCatalog, CompositionOptions.DisableSilentRejection);
        }

        #region Model Initializaton for GUI
        //[Import()]
        //public ExportFactory<MainModel> MainModelFactory { get; set; }

        //public MainModelTests()
        //{
        //    Container.ComposeParts(this);
        //}

        [TestMethod]
        public void SingletonTest()
        {
            var mainModel1 = MainModel.Instance;
            var mainModel2 = MainModel.Instance;
            Assert.AreEqual(mainModel1, mainModel2);
        }
        #endregion Model Initializaton for GUI

        //[ClassCleanup]
        //public void CleanUp()
        //{
        //    MainModel.Instance.Dispose();
        //}
    }
}
