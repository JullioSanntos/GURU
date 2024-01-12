using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMEA.Configuration;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;

namespace GURU.Model
{
    [JsonConverter(typeof(ConfiguredTypesConverter))]
    public class InitialStressType : ConfiguredBindableEntityBase<InitialStressType, CStressType>
    {

        //public InitialStressType(string id) : base(id) { }

        #region constructors
        internal InitialStressType(CStressType configuredStressType) : base(configuredStressType.Id.ToString())
        {
            Initialize(configuredStressType);
        }
        #endregion constructors

        private void Initialize(CStressType configuredStress)
        {
            SourceConfigurationdObject = configuredStress;
            Name = configuredStress.Name;
            Description = configuredStress.Description;
        }
    }
}
