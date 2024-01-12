using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Model
{
    // ReSharper disable once InconsistentNaming
    public class DI
    {
        public static Dictionary<Type,CreationPolicy> CreationPolicyPerType { get; } = new Dictionary<Type, CreationPolicy>();
        public static RegistrationBuilder AddRegistrations(RegistrationBuilder registrationBuilder, CreationPolicy defaultCreationPolicy = CreationPolicy.Shared)
        {
            if (CreationPolicyPerType.ContainsKey(typeof(MainModel)))
                registrationBuilder.ForType<MainModel>().Export<MainModel>().SetCreationPolicy(CreationPolicyPerType[typeof(MainModel)]);
            else
                registrationBuilder.ForType<MainModel>().Export<MainModel>().SetCreationPolicy(defaultCreationPolicy);

            return registrationBuilder;
        }
    }
}
