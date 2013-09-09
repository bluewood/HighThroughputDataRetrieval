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

        #region Properties
        #region DisplayName
        public override string DisplayName
        {
            get { return "Summary"; }
        }
        #endregion // DisplayName

        #region ProteinID
        // ReSharper disable once InconsistentNaming
        public string ProteinID
        {
            get { return Input.ProteinID; }
        }
        #endregion // ProteinID

        #region Organism
        public string Organism
        {
            get { return Input.Organism; }
        }
        #endregion // Organism

        #region Keyword
        public string Keyword
        {
            get { return Input.Keyword; }
        }
        #endregion // Keyword
        #endregion // Properties

        #region Commands
        #region RetrievalCountCommand
        /// <summary>
        /// Returns the command which, when executed, connects to NCBI and retrieval hit count table
        /// </summary>
        public ICommand RetrievalCountCommand
        {
            get {
                return _retrievalCountCommand ?? (_retrievalCountCommand = new RelayCommand(ConnectPubMed));
            }
        }

        // Display hit count table by NCBI handler
        public void ConnectPubMed()
        {
// ReSharper disable InconsistentNaming
            HitCountTableViewModel HitCountVM = new HitCountTableViewModel(ProteinID, Organism, Keyword);
// ReSharper restore InconsistentNaming
            HitCountTableView hitView = new HitCountTableView {DataContext = HitCountVM};
            HitCountVM.GetCountNumber();
            hitView.Show();
        }
        #endregion // RetrievalCountCommand
        #endregion // Commands

        #region Methods
        internal override bool IsValid()
        {
            return true;
        }
        #endregion // IsValid
    }
}
