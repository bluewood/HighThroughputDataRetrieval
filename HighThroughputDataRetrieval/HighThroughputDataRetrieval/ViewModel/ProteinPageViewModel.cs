using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using Microsoft.Win32;
using System.IO;

namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// The wizard page that allows the user to input 
    /// identifiers for searching.
    /// </summary>
    public class ProteinPageViewModel : InputWizardViewModelBase
    {
        #region Fields
        RelayCommand _loadProteinCommand;
        RelayCommand _openFileCommand;
        #endregion // Fields

        #region Constructor
        public ProteinPageViewModel(UserInput input): base(input)
        {
        }
        #endregion // Constructor

        #region Commands
        #region LoadIdentifierCommand
        /// <summary>
        /// Returns the command which, when executed, load example identifiers
        /// into textbox.
        /// </summary>
        public ICommand LoadIdentifierCommand
        {
            get { return _loadProteinCommand ?? (_loadProteinCommand = new RelayCommand(LoadData)); }
        }

        /// <summary>
        /// Load sample data into textbox.
        /// </summary>
        public void LoadData()
        {
            Input.ProteinID = "LYNX1\nSPARCL1\nSCG3CRTAC1\nPARK7\nTGFB\ngag"; 
            base.OnPropertyChanged("ProteinID");
        }
        #endregion // LoadIdentifierCommand

        #region OpenFileCommand
        /// <summary>
        /// Returns the command which, when executed, runs OpenFileDialog
        /// </summary>
        public ICommand OpenFileCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand(OpenFile)); }
        }

        public void OpenFile()
        {
            OpenFileDialog ofd1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 1,
                Multiselect = false
            };

            // Selecting multiple files not allowed

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = ofd1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                using (StreamReader sr = new StreamReader(ofd1.FileName))
                {
                    Input.ProteinID = sr.ReadToEnd();
                    base.OnPropertyChanged("ProteinID");
                }
            }
        }
        #endregion // OpenFileCommand
        #endregion // Commands

        #region Properties
        #region ProteinID
// ReSharper disable InconsistentNaming
        public string ProteinID
// ReSharper restore InconsistentNaming
        {
            get { return Input.ProteinID; }
            set
            {   
                Input.ProteinID = value;
                base.OnPropertyChanged("ProteinID");
            }
        }
        #endregion // ProteinID

        #region DisplayName
        public override string DisplayName
        {
            get { return "Identifiers"; }
        }
        #endregion // DisplayName
        #endregion // Properties

        #region Methods
        internal override bool IsValid()
        {
            return true;
        }
        #endregion // Methods
    }
}
