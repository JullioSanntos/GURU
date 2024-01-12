using System.Threading.Tasks;
using GURU.Common;
using GURU.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests1.Commom
{
    [TestClass]
    public class DialogViewModelTests
    {

        [TestMethod]
        public void InitializationTest()
        {
            var target = new DialogViewModel();
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void DialogSelectionEnumTest()
        {
            var target = DialogSelection.OK;
            Assert.AreEqual(1, (int)target);
        }

        [TestMethod]
        public async Task ParseObjectEnumTest()
        {
            var target = new DialogViewModel();
            var propertyChanged = new TaskCompletionSource<bool>();
            target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(target.CurrentSelection))
                    propertyChanged.TrySetResult(true);
            };
            target.ParseSelection(DialogSelection.OK);
            var actual = target.CurrentSelection;
            Assert.AreEqual(DialogSelection.OK, actual);
            await Task.WhenAny(propertyChanged.Task, Task.Delay(500)) ;
            Assert.IsTrue(propertyChanged.Task.Result);
        }

        [TestMethod]
        public void ParseObjectStringTest()
        {
            var target = new DialogViewModel();
            target.ParseSelection("1");
            var actual = target.CurrentSelection;
            Assert.AreEqual(DialogSelection.OK, actual);
        }

    }
}
