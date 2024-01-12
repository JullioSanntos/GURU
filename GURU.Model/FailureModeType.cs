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
    public class FailureModeType : ConfiguredBindableEntityBase<FailureModeType, CFailureModeType>
    {

        #region IPound
        private string _iPound;

        public string IPound
        {
            get { return _iPound; }
            set { SetProperty(ref _iPound, value); }
        }
        #endregion IPound

        #region PrimaryFunction
        private string _primaryFunction;

        public string PrimaryFunction
        {
            get { return _primaryFunction; }
            set { SetProperty(ref _primaryFunction, value); }
        }
        #endregion PrimaryFunction

        #region EnergyType
        private string _energyType;

        public string EnergyType
        {
            get { return _energyType; }
            set { SetProperty(ref _energyType, value); }
        }
        #endregion EnergyType

        #region constructors
        internal FailureModeType(CFailureModeType configuredFailureMode) : base(configuredFailureMode.Id.ToString())
        {
            Initialize(configuredFailureMode);
        }
        #endregion constructors

        private void Initialize(CFailureModeType configuredFailureMode)
        {
            SourceConfigurationdObject = configuredFailureMode;
            Name = configuredFailureMode.Name;
            Description = configuredFailureMode.Description;
            IPound = configuredFailureMode.Ipound;
            PrimaryFunction = configuredFailureMode.PrimaryFunction;
            EnergyType = configuredFailureMode.EnergyType;
        }
    }
}
