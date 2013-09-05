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
using Microsoft.Win32;
using System.Windows;
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
            get
            {
                if (_loadProteinCommand == null)
                    _loadProteinCommand = new RelayCommand(() => this.LoadData());

                return _loadProteinCommand;
            }
        }

        /// <summary>
        /// Load sample data into textbox.
        /// </summary>
        public void LoadData()
        {
            base.Input.ProteinID = "LYNX1\nSPARCL1\nSCG3CRTAC1\nPARK7\nTGFB\ngag"; 
            base.OnPropertyChanged("ProteinID");
        }
        #endregion // LoadIdentifierCommand

        #region OpenFileCommand
        /// <summary>
        /// Returns the command which, when executed, runs OpenFileDialog
        /// </summary>
        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new RelayCommand(() => this.OpenFile());

                return _openFileCommand;
            }
        }

        public void OpenFile()
        {
            OpenFileDialog ofd1 = new OpenFileDialog();

            ofd1.InitialDirectory = "c:\\" ;
            ofd1.Filter = "txt files (*.txt)|*.txt" ;
            ofd1.FilterIndex = 1;

            // Selecting multiple files not allowed
            ofd1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = ofd1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                try
                {
                    using (StreamReader sr = new StreamReader(ofd1.FileName))
                    {
                        base.Input.ProteinID = sr.ReadToEnd();
                        base.OnPropertyChanged("ProteinID");
                    }
                }
                catch (Exception ex)
                {
                    //"Could not read the file";
                }
            }
        }
        #endregion // OpenFileCommand
        #endregion // Commands

        #region Properties
        #region ProteinID
        public string ProteinID
        {
            get { return base.Input.ProteinID; }
            set
            {   
                base.Input.ProteinID = value;
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
