using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GURU.Common.XAMLExtensions;
using GURU.Model;

namespace GURU.ViewsAndViewModels
{
    public class InterfacesListViewModel
    {
        #region AddNewInterfaceRecordCommand
        public ICommand AddNewInterfaceRecordCommand { get { return new RelayCommand(AddNewInterfaceRecord, CanAddNewInterfaceRecord); } }
        public void AddNewInterfaceRecord(object arg) { MainModel.MainModelInstance.Interfaces.Add(new Interface());  }
        public bool CanAddNewInterfaceRecord(object arg) { return true; }
        #endregion AddNewInterfaceRecordCommand

    }
}
