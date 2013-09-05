﻿using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;


namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// The wizard page that allows the user to input 
    /// organism name for searching.
    /// </summary>
    public class OrganismPageViewModel : InputWizardViewModelBase
    {
        #region Fieldss
        RelayCommand _loadOrgCommand;
        #endregion // Fields

        #region Constructor
        public OrganismPageViewModel(UserInput input)
            : base(input)
        {
        }
        #endregion // Constructor

        #region Commands
        #region LoadOrgCommand
        /// <summary>
        /// Returns the command which, when executed, load example Organism
        /// into textbox.
        /// </summary>
        public ICommand LoadOrgCommand
        {
            get
            {
                if (_loadOrgCommand == null)
                    _loadOrgCommand = new RelayCommand(() => this.LoadData());

                return _loadOrgCommand;
            }
        }

        /// <summary>
        /// Load sample data into textbox.
        /// </summary>
        void LoadData()
        {
            base.Input.Organism = "Human";
            base.OnPropertyChanged("Organism");
        }
        #endregion // LoadOrgCommand
        #endregion // Commands

        #region Properties
        #region Organism
        public string Organism
        {
            get { return Input.Organism; }
            set
            {
                Input.Organism = value;
                base.OnPropertyChanged("Organism");
            }
        }
        #endregion // Organism

        #region DisplayName
        public override string DisplayName
        {
            get { return "Organism"; }
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
