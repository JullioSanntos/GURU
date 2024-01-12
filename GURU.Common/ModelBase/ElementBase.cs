namespace GURU.Common.ModelBase
{
    public abstract class ElementBase : BindableBase, IElement
    {

        #region Name
        private string _name;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion Name

    }
}
