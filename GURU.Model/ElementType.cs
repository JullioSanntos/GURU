using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FMEA.Configuration;
using GURU.Common;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;

namespace GURU.Model
{
    [JsonConverter(typeof(ConfiguredTypesConverter))]

    public sealed class ElementType : ConfiguredBindableEntityBase<ElementType, CElementType>, IComparable<ElementType>
    {

        public override CElementType SourceConfigurationdObject
        {
            get;
            internal set;
        }

        #region constructors

        internal ElementType(FMEA.Configuration.CElementType configuredElementType) 
            : base(configuredElementType?.Id.ToString() ?? string.Empty) 
        {
            Initialize(configuredElementType);
        }

        private void Initialize(FMEA.Configuration.CElementType configuredElementType)
        {
            Name = configuredElementType?.Name;
            Description = configuredElementType?.Description ?? "null ElementType";
            SourceConfigurationdObject = configuredElementType;
        }
        #endregion constructors

        public ElementType GetClone()
        {
            var elemType = new ElementType(SourceConfigurationdObject);
            return elemType;
        }

        #region Comparison
        public override bool Equals(object obj)
        {
            var otherElemType = obj as ElementType;
            return this.Name == otherElemType?.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public int CompareTo(ElementType other)
        {
            // ReSharper disable once StringCompareIsCultureSpecific.1
            return string.Compare(this.Name, other.Name);
        }
        #endregion Comparison
    }
}
