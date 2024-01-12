using System;
using System.CodeDom;
using GURU.Common;
using Newtonsoft.Json;

namespace GURU.Model.Interfaces
{
    public abstract class BindableEntityBase : BindableBase
    {
        public const string NullName = "<none>";

        #region properties
        #region Id
        private string _id;
        public string Id
        {
            get { return _id ?? ( _id = GetHashCode().ToString() ); }
            protected set { _id = value; }
        }
        #endregion Id

        #region Name
        private string _name;
        public virtual string Name
        {
            get { return _name ?? (_name = GetHashCode().ToString() ); }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Description
        private string _description;

        public virtual string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        #endregion Description
        #endregion properties

        #region constructors
        protected BindableEntityBase(string id = null)
        {
            if (id == null) return;
            Id = id;
        }
        #endregion constructors

    }

    [Serializable]
    public abstract class BindableEntityBase<T> : BindableEntityBase, IComparable<BindableEntityBase<T>> where T : BindableEntityBase
    {
        #region properties
        #region Id
        private string _id;
        public new string Id
        {
            get { return _id ?? (_id = this.GetHashCode().ToString()); }
            protected set { _id = value; }
        }
        #endregion Id

        #region Name
        private string _name;
        public override string Name
        {
            get { return _name ?? (_name = EntityBase?.Name ??  NullName); }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region EntityBase
        private T _entityBase;
        [JsonIgnore]
        public T EntityBase
        {
            get { return _entityBase; }
            set { SetProperty(ref _entityBase, value);}
        }
        #endregion EntityBase

        #region EntityBaseType
        [JsonIgnore]
        public Type EntityBaseType
        {
            get { return typeof(T); }
        }
        #endregion EntityBaseType
        #endregion properties

        #region constructors
        protected BindableEntityBase(string id) : base(id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }
        #endregion constructors

        #region methods
        //#region Equals override
        //public override bool Equals(object obj)
        //{
        //    var otherElemType = obj as BindableEntityBase<T>;
        //    return this.Id == otherElemType?.Id;
        //}

        //public override int GetHashCode()
        //{
        //    return Id.GetHashCode();
        //}
        //#endregion Equals override

        #region Comparison
        public int CompareTo(BindableEntityBase<T> other)
        {
            // ReSharper disable once StringCompareIsCultureSpecific.1
            return string.Compare(this.Name, other.Name);
        }
        #endregion Comparison
        #endregion methods

    }
}
