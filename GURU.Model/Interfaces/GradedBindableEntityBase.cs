using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common.Interfaces;
using Newtonsoft.Json;

namespace GURU.Model.Interfaces
{
    public abstract class GradedBindableEntityBase<T> : BindableEntityBase<T>, IGradedEntity, IValidate, IAbbreviate, IComparable<GradedBindableEntityBase<T>> where T : BindableEntityBase
    {
        #region Grade
        private int _grade;
        public int Grade
        {
            get { return _grade; }
            set { SetProperty(ref _grade, value); }
        }
        #endregion Grade

        #region ValidGrades
        [JsonIgnore]
        public List<int> ValidGrades { get { return StaticValidGrades; } }
        public static List<int> StaticValidGrades { get { return new List<int>() { 0, 1, 2, 3, 4, 5 }; } }
        #endregion ValidGrades

        #region Abbreviation
        [JsonIgnore]
        public string Abbreviation { get { return $@"{Grade} {Name}"; } }
        #endregion Abbreviation

        #region IValidate

        private Func<bool> _isValidFunc;
        [JsonIgnore]
        public virtual Func<bool> IsValidFunc
        {
            get { return _isValidFunc ?? (_isValidFunc = () => Grade > 0 && Grade < 6); }
            set { SetProperty(ref _isValidFunc, value); }
        }
        #region IsValid
        [JsonIgnore]
        public bool IsValid { get { return IsValidFunc(); } }

        #endregion IsValid
        #endregion IValidate

        #region constructors
        protected GradedBindableEntityBase(T entityBase) : base(entityBase.Id)
        {
            EntityBase = entityBase;
        }
        #endregion constructors

        #region IComparable
        public int CompareTo(GradedBindableEntityBase<T> other)
        {
            // ReSharper disable once StringCompareIsCultureSpecific.1
            return string.Compare(this.Name, other.Name);
        }
        #endregion IComparable

        //#region "Equals" override
        //public override bool Equals(object obj)
        //{
        //    return this.EntityBase.Id == (obj as GradedBindableEntityBase<T>)?.EntityBase.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return this.EntityBase.Id.GetHashCode();
        //}
        //#endregion "Equals" override

        #region ToString
        public override string ToString()
        {
            return Abbreviation;
        }
        #endregion ToString
    }
    
}
