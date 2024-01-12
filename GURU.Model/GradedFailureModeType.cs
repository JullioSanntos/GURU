using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;

namespace GURU.Model
{
    [JsonConverter(typeof(GradedTypesConverter))]
    public class GradedFailureModeType : GradedBindableEntityBase<FailureModeType>, IGetClone<GradedFailureModeType>
    {
        #region properties
        #endregion properties

        #region constructors
        public GradedFailureModeType(FailureModeType failureModeType) : base(failureModeType) { }

        #endregion constructors

        #region methods

        #region GetClone
        public GradedFailureModeType GetClone(int? grade)
        {
            var cond = new GradedFailureModeType(this.EntityBase) { Grade = grade ?? this.Grade };
            return cond;
        }
        #endregion GetClone
        #endregion methods
    }
}
