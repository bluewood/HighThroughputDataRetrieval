using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using HighThroughputDataRetrievalBackend.Model;

namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// Abstract base class for all pages shown in the wizard.
    /// </summary>
    public abstract class InputWizardViewModelBase : INotifyPropertyChanged
    {
        #region Fields
        public UserInput Input;
        private bool _isCurrentPage;
        #endregion // Fields

        #region Constructor

        protected InputWizardViewModelBase(UserInput input)
        {
            Input = input;
        }
        #endregion // Constructor

        #region Properties

        
        public abstract string DisplayName { get; }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                OnPropertyChanged("IsCurrentPage");
            }
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        #endregion // Methods

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion // INotifyPropertyChanged Members
    }
}
