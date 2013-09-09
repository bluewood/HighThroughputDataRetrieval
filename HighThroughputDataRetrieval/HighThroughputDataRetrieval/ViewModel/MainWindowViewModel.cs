using System.Windows.Input;
using HighThroughputDataRetrieval.View;

namespace HighThroughputDataRetrieval.ViewModel
{
    public class MainWindowViewModel
    {
        #region Fields
        RelayCommand _searchPubMedCommand;
        #endregion // Fields

        #region Commands
        #region SearchPubMedCommand
        /// <summary>
        /// Returns the command which, when executed, runs wizard to get user input
        /// </summary>
        public ICommand SearchPubMedCommand
        {
            get { return _searchPubMedCommand ?? (_searchPubMedCommand = new RelayCommand(OpenWizard)); }
        }

        public void OpenWizard()
        {
            InputWizardDialog dlg = new InputWizardDialog();
            dlg.ShowDialog();
        }
        #endregion // SearchPubMedCommand
        #endregion Commands
    }
}
