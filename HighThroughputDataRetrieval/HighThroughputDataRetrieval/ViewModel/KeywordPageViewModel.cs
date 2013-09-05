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
    /// The wizard page that allows the user to input 
    /// optional keywords for searching.
    /// </summary>
    class KeywordPageViewModel : InputWizardViewModelBase
    {
        #region Fields
        //RelayCommand _addKeywordCommand;
        RelayCommand _loadKeywordCommand;
        #endregion // Fields

        #region Constructor
        public KeywordPageViewModel(UserInput input)
            : base(input)
        {
        }
        #endregion // Constructor

        #region Commands
       // #region AddKeywordCommand
       // /// <summary>
       // /// Returns the command which, when executed, add a single Keyword
       // /// into textblock.
       // /// </summary>
       // public ICommand AddKeywordCommand
       // {
       //     get
       //     {
       //         if (_addKeywordCommand == null)
       //             _addKeywordCommand = new RelayCommand(() => this.AddKeyword());

       //         return _addKeywordCommand;
       //     }
       // }

       // /// <summary>
       // /// Load sample data into textbox.
       // /// </summary>
       //public void AddKeyword()
       // {
       //    Keywords.Add("hello testing");
       //   // Keywords.Add(this.Keyword);
       //    OnPropertyChanged("Keywords");
       // }
       // #endregion

        #region LoadKeywordCommand
        /// <summary>
        /// Returns the command which, when executed, load example Keyword
        /// into textbox.
        /// </summary>
        public ICommand LoadKeywordCommand
        {
            get
            {
                if (_loadKeywordCommand == null)
                    _loadKeywordCommand = new RelayCommand(() => this.LoadData());

                return _loadKeywordCommand;
            }
        }

        /// <summary>
        /// Load sample data into textbox.
        /// </summary>
        public void LoadData()
        {
            base.Input.Keyword = "HIV";
            base.OnPropertyChanged("Keyword");
        }
        #endregion // LoadKeywordCommand
        #endregion // Commands

        #region Properties
        #region Keyword
        public string Keyword
        {
            get
            {   return base.Input.Keyword; }
            set
            {
                base.Input.Keyword = value;
                base.OnPropertyChanged("Keyword");
            }
        }
        #endregion // Keyword

        #region DisplayName
        public override string DisplayName
        {
            get { return "Keywords"; }
        }
        #endregion
        #endregion // Properties

        #region Methods
        internal override bool IsValid()
        {
            return true;
        }
        #endregion // Methods
    }
}
