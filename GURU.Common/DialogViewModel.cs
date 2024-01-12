using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GURU.Common.Interfaces;
using GURU.Common.XAMLExtensions;

namespace GURU.Common
{

    public class DialogViewModel : BindableBase
    {

        #region IsVisible
        private bool _isVisible;

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }
        #endregion IsVisible

        #region IsBlocking
        private bool _isBlocking = true;
        public bool IsBlocking
        {
            get { return _isBlocking; }
            set { SetProperty(ref _isBlocking, value); }
        }
        #endregion IsBlocking

        #region Title
        private string _title;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion Title

        #region Message
        private string _message;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        #endregion Message

        #region Messages
        private List<string> _messages;

        public List<string> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }
        #endregion Messages

        #region CurrentSelection
        private DialogSelection _currentSelection;
        public DialogSelection CurrentSelection
        {
            get { return _currentSelection; }
            set
            {
                SetProperty(ref _currentSelection, value);
            }
        }
        #endregion CurrentSelection


        #region FadeOutSeconds
        private double _fadeOutSeconds;

        public double FadeOutSeconds
        {
            get { return _fadeOutSeconds; }
            set { SetProperty(ref _fadeOutSeconds, value); }
        }
        #endregion FadeOutSeconds


        #region CommandsList
        private List<DialogSelection> _showingSelections = new List<DialogSelection>() { DialogSelection.OK };

        public List<DialogSelection> CommandsList
        {
            get { return _showingSelections; }
            set { SetProperty(ref _showingSelections, value); }
        }
        #endregion CommandsList

        #region SelectionCompletedCommand
        public ICommand SelectionCompletedCommand { get { return new RelayCommand(ParseSelection); } }

        public void ParseSelection(object dialogSelection)
        {
            var prevSelection = CurrentSelection;
            // argument can be "enum DialogSelection" or its equivalent integer value
            if (dialogSelection is DialogSelection) _currentSelection = (DialogSelection)dialogSelection;
            else Enum.TryParse((string)dialogSelection, true, out _currentSelection);
            if (prevSelection != CurrentSelection) RaisePropertyChanged(nameof(CurrentSelection));
        }
        #endregion SelectionCompletedCommand

        #region ShowDialogCommand
        public ICommand ShowDialogCommand { get { return new RelayCommand((a) => ShowDialog(a as double?));} }
        public void ShowDialog(double? fadeOut = null)
        {
            IsBlocking = true;
            if (fadeOut != null) FadeOutSeconds = (double)fadeOut;
            IsVisible = true;
        }
        #endregion ShowDialogCommand

        #region ShowCommand
        public ICommand ShowCommand { get { return new RelayCommand((a) => Show(a as double?)); } }
        public void Show(double? fadeOut = null)
        {
            IsBlocking = false;
            if (fadeOut != null) FadeOutSeconds = (double)fadeOut;
            IsVisible = true;
        }
        #endregion ShowDialog
    }
}
