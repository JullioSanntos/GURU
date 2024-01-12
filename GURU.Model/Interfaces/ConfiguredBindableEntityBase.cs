using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GURU.Model.Interfaces
{
    
    public class ConfiguredBindableEntityBase<T, TC> : BindableEntityBase<T> where T : BindableEntityBase
    {
        #region SourceConfigurationdObject
        private TC _sourceConfigurationdObject;

        [JsonIgnore]
        public virtual TC SourceConfigurationdObject
        {
            get { return _sourceConfigurationdObject; }
            internal set { SetProperty(ref _sourceConfigurationdObject, value); }
        }
        #endregion SourceConfigurationdObject

        #region constructors
        public ConfiguredBindableEntityBase(string id) : base(id) { }
        #endregion constructors
    }
}
