using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Model;

namespace GURU
{
    // ReSharper disable once InconsistentNaming
    public class DI
    {
        public static RegistrationBuilder AddRegistrations(RegistrationBuilder registrationBuilder, CreationPolicy creationPolicy = CreationPolicy.Shared)
        {
            return registrationBuilder;
        }
    }
}
