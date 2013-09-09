using System;
using System.Collections.Generic;
using HighThroughputDataRetrievalBackend.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HighThroughputDataRetrieval.ViewModel
{
    /// <summary>
    /// The main ViewModel class is for the input wizard.
    /// This class contains the various pages shown in the 
    /// workflow and provides navigation between the pages.
    /// In addition, the instance of Input is created here
    /// before passing to various view models.
    /// </summary>
    public class InputWizardViewModel:INotifyPropertyChanged 
    {
        #region Fields
        UserInput _pageInput;
        RelayCommand _cancelCommand;
        RelayCommand _moveNextCommand;
        RelayCommand _movePreviousCommand;
        InputWizardViewModelBase _currentPage;
        ObservableCollection<InputWizardViewModelBase> _pages;
        #endregion // Fields
        
        #region Constructor
        /// <summary>
        /// Constructs the default instance of Input and set
        /// current page index to 0.
        /// </summary>
        public InputWizardViewModel()
        {
            _pageInput = new UserInput();
            CurrentPage = Pages[0];
        }
        #endregion // Constructor

        #region Commands
        #region CancelCommand
        /// <summary>
        /// Returns the command which, when executed, cancels the search 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand(CancelSearch)); }
        }

        void CancelSearch()
        {   
            _pageInput = null;
            OnRequestClose();
        }
        #endregion // CancelCommand

        #region MovePreviousCommand
        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                return _movePreviousCommand ?? (_movePreviousCommand = new RelayCommand(
                    MoveToPreviousPage,
                    () => CanMoveToPreviousPage));
            }
        }

        bool CanMoveToPreviousPage
        {
            get { return 0 < CurrentPageIndex; }
        }

        void MoveToPreviousPage()
        {
            if (CanMoveToPreviousPage)
                CurrentPage = Pages[CurrentPageIndex - 1];
        }
        #endregion // MovePreviousCommand

        #region MoveNextCommand
        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                return _moveNextCommand ?? (_moveNextCommand = new RelayCommand(
                    MoveToNextPage,
                    () => CanMoveToNextPage));
            }
        }

        bool CanMoveToNextPage
        {
            get { return CurrentPage != null && CurrentPage.IsValid(); }
        }

        void MoveToNextPage()
        {
            if (CanMoveToNextPage)
            {
                if (CurrentPageIndex < Pages.Count - 1)
                    CurrentPage = Pages[CurrentPageIndex + 1];
                else
                    OnRequestClose();
            }
        }
        #endregion // MoveNextCommand
        #endregion Commands

        #region Properties
        /// <summary>
        /// Returns input entered by user.
        /// If this returns null, the user cancelled the search.
        /// </summary>
        public UserInput Input
        {
            get { return _pageInput; }
        }

        /// <summary>
        /// Returns the page ViewModel that the user is currently viewing
        /// </summary>
        public InputWizardViewModelBase CurrentPage
        {
            get { return _currentPage; }
            private set
            {
                if (value == _currentPage)
                    return;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = false;

                _currentPage = value;

                if (_currentPage != null)
                    _currentPage.IsCurrentPage = true;

                OnPropertyChanged("CurrentPage");
                OnPropertyChanged("IsOnLastPage");
            }
        }

        /// <summary>
        /// Returns true if the user is currently viewing the last page 
        /// in the workflow.  This property is used by InputWizardView
        /// to switch the Next button's text to "Finish" when the user
        /// has reached the final page.
        /// </summary>
        public bool IsOnLastPage
        {
            get { return CurrentPageIndex == Pages.Count - 1; }
        }

        /// <summary>
        /// Returns a collection of all page ViewModels.
        /// </summary>
        public ObservableCollection<InputWizardViewModelBase> Pages
        {
            get
            {
                if (_pages == null)
                    CreatePages();

                return _pages;
            }
        }
        #endregion // Properties

        #region Events
        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;
        #endregion // Events

        #region Methods
        /// <summary>
        /// Creating pages by adding view models
        /// </summary>
        public void CreatePages()
        {
            var proteinViewModel  = new ProteinPageViewModel(Input);
            var organismViewModel = new OrganismPageViewModel(Input);
            var keywordViewModel  = new KeywordPageViewModel(Input);
            var summaryViewModel  = new SummaryPageViewModel(Input);
            var pages             = new List<InputWizardViewModelBase>
            {
                new WelcomePageViewModel(Input),
                proteinViewModel,
                organismViewModel,
                keywordViewModel,
                summaryViewModel
            };

            _pages = new ObservableCollection<InputWizardViewModelBase>(pages);
        }

        public int CurrentPageIndex
        {
            get
            {
                if (CurrentPage == null)
                {
                    //Debug.Fail("Why is the current page null?");
                }

                return Pages.IndexOf(CurrentPage);
            }
        }

        public void OnRequestClose()
        {
            EventHandler handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        #endregion // Methods

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion // INotifyPropertyChanged Members
    }
}
