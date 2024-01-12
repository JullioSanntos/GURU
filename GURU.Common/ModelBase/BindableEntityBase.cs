using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GURU.Common.ModelBase
{
    public abstract class BindableEntityBase : BindableBase
    {
        #region Name
        private string _id;
        public virtual string Id
        {
            get { return _id ?? ( _id = GetHashCode().ToString() ); }
        }
        #endregion Name

        #region Name
        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Description
        private string _description;

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        #endregion Description
    }

    public abstract class BindableEntityBase<T> : BindableBase
    {
        #region Name
        private string _id;
        public virtual string Id
        {
            get { return _id ?? (_id = GetHashCode().ToString()); }
        }
        #endregion Name

        #region Name
        private string _name;
        public virtual string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

        #region Description
        private string _description;

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        #endregion Description

        #region ConfiguredObject
        private T _configuredObject;

        public T ConfiguredObject
        {
            get { return _configuredObject; }
            set { SetProperty(ref _configuredObject, value); }
        }
        #endregion ConfiguredObject

    }
}
