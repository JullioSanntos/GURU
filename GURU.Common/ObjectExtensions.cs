using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common
{
    public static class ObjectExtensions
    {
        public static object CloneTo<T>(this object fromObject, object toObject) where T : class
        {
            var cloningFilterType = typeof(T);
            var toFields = toObject.GetType().FindAllFields().ToList();

            foreach (var fromField in cloningFilterType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var toField = toFields.FirstOrDefault(tf => tf.Name == fromField.Name);
                if (toField == null) continue;
                toField.SetValue(toObject, fromField.GetValue(fromObject));
            }

            return toObject;
        }

        public static object CloneTo(this object fromObject, object toObject)
        {
            var toFields = toObject.GetType().FindAllFields().ToList();
            foreach (var fromField in fromObject.GetType().FindAllFields())
            {
                var toField = toFields.FirstOrDefault(tf => tf.Name == fromField.Name);
                if (toField == null) continue;
                toField.SetValue(toObject, fromField.GetValue(fromObject));
            }

            return toObject;
        }

        public static IEnumerable<FieldInfo> FindAllFields(this Type t)
        {
            if (t == null) return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(FindAllFields(t.BaseType));
        }

 }
}
