using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;

namespace GURU.ViewsAndViewModels
{
    public class MenuItem : BindableBase
    {

        #region Header
        private string _myHeader;

        public string MyHeader
        {
            get { return _myHeader; }
            set { SetProperty(ref _myHeader, value); }
        }
        #endregion Header

        public MenuItem(string myHeader)
        {
            MyHeader = myHeader;
        }
    }
}
