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
    public class InitialConditionType : ConfiguredBindableEntityBase<InitialConditionType, CConditionType>
    {

        #region IsInitial
        private bool _isInitial;
        public bool IsInitial
        {
            get { return _isInitial; }
            set { SetProperty(ref _isInitial, value); }
        }
        #endregion IsInitial

        #region IsResulting
        private bool _isResulting;

        public bool IsResulting
        {
            get { return _isResulting; }
            set { SetProperty(ref _isResulting, value); }
        }
        #endregion IsResulting

        #region constructors
        internal InitialConditionType(CConditionType configuredConditionType) : base(configuredConditionType.Id.ToString())
        {
            Initialize(configuredConditionType);
        }
        #endregion constructors

        private void Initialize(CConditionType configuredCondition)
        {
            SourceConfigurationdObject = configuredCondition;
            Name = configuredCondition.Name;
            IsInitial = configuredCondition.IsInitial;
            Description = configuredCondition.Description;
            IsResulting = configuredCondition.IsResulting;
        }
    }
}
