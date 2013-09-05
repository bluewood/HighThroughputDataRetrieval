using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;
using System.Data;
using HighThroughputDataRetrieval.View;

namespace HighThroughputDataRetrieval.ViewModel
{
    public class MainWindowViewModel
    {
        #region Fields
        RelayCommand _searchPubMedCommand;
        #endregion // Fields

        #region Constructor
        public MainWindowViewModel()
        {
        }
        #endregion

        #region Commands
        #region SearchPubMedCommand
        /// <summary>
        /// Returns the command which, when executed, runs wizard to get user input
        /// </summary>
        public ICommand SearchPubMedCommand
        {
            get
            {
                if (_searchPubMedCommand == null)
                    _searchPubMedCommand = new RelayCommand(() => this.OpenWizard());

                return _searchPubMedCommand;
            }
        }
        #endregion // SearchPubMedCommand
        #endregion Commands

        #region Property
        public void OpenWizard()
        {
            InputWizardDialog dlg = new InputWizardDialog();
            dlg.ShowDialog();
        }
        #endregion Property
    }
}
