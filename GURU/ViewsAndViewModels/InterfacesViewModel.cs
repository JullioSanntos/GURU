using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GURU.Common;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class InterfacesViewModel : BindableBase
    {
        //#region Element1Elements
        //public List<Element> Element1Elements
        //{
        //    get
        //    {
        //        var result = MainModel.MainModelInstance.Elements.Where(e => MainModel.MainModelInstance.SelectedInterface?.Element1 != e).ToList();
        //        return result;
        //    }
        //}
        //#endregion Element1Elements


        //#region Element2Elements
        //public List<Element> Element2Elements
        //{
        //    get { return MainModel.MainModelInstance.Elements.Where(e => MainModel.MainModelInstance.SelectedInterface?.Element2 != e).ToList(); }
        //}

        //public InterfacesViewModel ViewModel { get; set; }

        //#endregion Element2Elements

        //public InterfacesViewModel()
        //{
        //    MainModel.MainModelInstance.PropertyChanged += MainModelInstance_PropertyChanged;
        //}

        //private void MainModelInstance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    RaisePropertyChanged(nameof(Element1Elements));
        //    RaisePropertyChanged(nameof(Element2Elements));
        //}

    }
}
