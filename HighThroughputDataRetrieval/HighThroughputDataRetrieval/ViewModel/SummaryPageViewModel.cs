using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using HighThroughputDataRetrieval.View;
using HighThroughputDataRetrievalBackend.Model;

namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// Summarize user input and provide search option to user
    /// </summary>
    public class SummaryPageViewModel : InputWizardViewModelBase
    {
        #region Fields
        RelayCommand _retrievalCountCommand; 
        #endregion // Fields

        #region Constructor
        public SummaryPageViewModel(UserInput input)
            : base(input)
        {
        }
        #endregion // Constructor

        #region Commands
        #region RetrievalCountCommand
        /// <summary>
        /// Returns the command which, when executed, connects to NCBI and retrieval hit count table
        /// </summary>
        public ICommand RetrievalCountCommand
        {
            get
            {
                if (_retrievalCountCommand == null)
                    _retrievalCountCommand = new RelayCommand(() => this.ConnectPubMed());

                return _retrievalCountCommand;
            }
        }

        // Display hit count table by NCBI handler
        public void ConnectPubMed()
        {
            HitCountTableViewModel HitCountVM = new HitCountTableViewModel(ProteinID, Organism, Keyword);
            HitCountTableView hitView = new HitCountTableView();
            hitView.DataContext = HitCountVM;
            HitCountVM.GetCountNumber();
            hitView.Show();
        }
        #endregion // RetrievalCountCommand
        #endregion // Commands

        #region Properties
        #region DisplayName
        public override string DisplayName
        {
            get { return "Summary"; }
        }
        #endregion // DisplayName

        #region ProteinID
        public string ProteinID
        {
            get {   return base.Input.ProteinID;}
        }
        #endregion // ProteinID

        #region Organism
        public string Organism
        {
            get { return base.Input.Organism; }
        }
        #endregion // Organism

        #region Keyword
        public string Keyword
        {
            get { return base.Input.Keyword; }
        }
        #endregion // Keyword
        #endregion // Properties

        #region Methods
        internal override bool IsValid()
        {
            return true;
        }
        #endregion // IsValid
    }
}
