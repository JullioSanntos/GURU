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
    public class GradedInitialStressType : GradedBindableEntityBase<InitialStressType>, IGetClone<GradedInitialStressType>
    {
        #region properties
        #endregion properties

        #region constructors
        public GradedInitialStressType(InitialStressType initialStressType) : base(initialStressType) { }
        #endregion constructors

        #region methods

        #region IsValidFunc
        private Func<bool> _isValidFunc;
        [JsonIgnore]
        public override Func<bool> IsValidFunc
        {
            get { return _isValidFunc ?? (_isValidFunc = () => Grade >= 0 && Grade < 6); }
            set { SetProperty(ref _isValidFunc, value); }
        }
        #endregion IsValidFunc

        #region GetClone
        public GradedInitialStressType GetClone(int? grade)
        {
            var cond = new GradedInitialStressType(this.EntityBase) { Grade = grade ?? this.Grade };
            return cond;
        }
        #endregion GetClone

        #endregion methods
    }
}
