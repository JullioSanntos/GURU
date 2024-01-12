using GURU.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GURU.Tests1.Commom
{
    [TestClass]
    public class ObjectExtensionTests
    {
        [TestMethod]
        public void GenericCloneToTest()
        {
            var fromObject = new FromObject() {AutoImplementedProperty = "AIP", PropertyWithBackingField = 123, NotPairedType = 234, BaseProperty = "base" };
            var toObject = new ToObject();
            Assert.AreNotEqual(fromObject.PropertyWithBackingField, toObject.PropertyWithBackingField);
            Assert.AreNotEqual(fromObject.AutoImplementedProperty, toObject.AutoImplementedProperty);
            Assert.AreNotEqual(fromObject.NotPairedType, toObject.NotPairedType);
            Assert.AreNotEqual(fromObject.BaseProperty, toObject.BaseProperty);
            fromObject.CloneTo<FromObject>(toObject);
            Assert.AreEqual(fromObject.PropertyWithBackingField, toObject.PropertyWithBackingField);
            Assert.AreEqual(fromObject.AutoImplementedProperty, toObject.AutoImplementedProperty);
            Assert.AreEqual(fromObject.NotPairedType, toObject.NotPairedType);
            Assert.AreNotEqual(fromObject.BaseProperty, toObject.BaseProperty);
        }

        [TestMethod]
        public void NonGenericCloneToTest()
        {
            var fromObject = new FromObject() { AutoImplementedProperty = "AIP", PropertyWithBackingField = 123, NotPairedType = 234, BaseProperty = "base"};
            var toObject = new ToObject();
            Assert.AreNotEqual(fromObject.PropertyWithBackingField, toObject.PropertyWithBackingField);
            Assert.AreNotEqual(fromObject.AutoImplementedProperty, toObject.AutoImplementedProperty);
            Assert.AreNotEqual(fromObject.NotPairedType, toObject.NotPairedType);
            Assert.AreNotEqual(fromObject.BaseProperty, toObject.BaseProperty);
            fromObject.CloneTo(toObject);
            Assert.AreEqual(fromObject.PropertyWithBackingField, toObject.PropertyWithBackingField);
            Assert.AreEqual(fromObject.AutoImplementedProperty, toObject.AutoImplementedProperty);
            Assert.AreEqual(fromObject.NotPairedType, toObject.NotPairedType);
            Assert.AreEqual(fromObject.BaseProperty, toObject.BaseProperty);
        }

        class ObjectBase
        {

            #region BaseProperty
            public string BaseProperty { get; set; }
            #endregion BaseProperty
        }

        class FromObject : ObjectBase
        {

            #region AutoImplementedProperty
            public string AutoImplementedProperty { get; set; }
            #endregion AutoImplementedProperty


            #region PropertyWithBackingField
            private int propertyWithBackingField;
            public int PropertyWithBackingField
            {
                get { return propertyWithBackingField; }
                set { propertyWithBackingField = value; }
            }
            #endregion PropertyWithBackingField


            #region NotPairedProperty
            public double NotPairedProperty { get; set; }
            #endregion NotPairedProperty


            #region NotPairedType
            public int NotPairedType { get; set; }
            #endregion NotPairedType
        }

        class ToObject : ObjectBase
        {

            #region AutoImplementedProperty
            public string AutoImplementedProperty { get; set; }
            #endregion AutoImplementedProperty


            #region PropertyWithBackingField
            private int propertyWithBackingField;
            public int PropertyWithBackingField
            {
                get { return propertyWithBackingField; }
                set { propertyWithBackingField = value; }
            }
            #endregion PropertyWithBackingField

            #region NotPairedType
            public double NotPairedType { get; set; }
            #endregion NotPairedType
        }
    }
}
