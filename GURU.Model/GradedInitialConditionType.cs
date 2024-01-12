using System;
using GURU.Common.Interfaces;
using GURU.Model.Interfaces;
using GURU.Model.JSON.Converters;
using Newtonsoft.Json;

namespace GURU.Model
{
    [JsonConverter(typeof(GradedTypesConverter))]

    public class GradedInitialConditionType : GradedBindableEntityBase<InitialConditionType>, IGetClone<GradedInitialConditionType>
    {
        #region properties
        #endregion properties

        #region constructors
        public GradedInitialConditionType(InitialConditionType initialCondType) : base(initialCondType) { }

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
        public GradedInitialConditionType GetClone(int? grade)
        {
            var cond = new GradedInitialConditionType(this.EntityBase) { Grade = grade ?? this.Grade };
            return cond;
        }
        #endregion GetClone

        #endregion methods

    }
}
