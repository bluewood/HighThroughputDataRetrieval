using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using Microsoft.Win32;

namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        public UserInput UserInputFromModel;
        //
        // Hyesun added for GetCount
        public NcbiDataRetrieval PubMedSearch { get; set; }
        private int _count;
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        //
        //
        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel()      
        {
            UserInputFromModel  = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountListWithProteins = new ObservableCollection<HitCountTable>();
        }

        #endregion // Constructor

        #region Commands
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
            var ofd1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 1,
                Multiselect = false
            };

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = ofd1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                using (var sr = new StreamReader(ofd1.FileName))
                {
                    UserInputFromModel.ProteinInModel = sr.ReadToEnd();
                    OnPropertyChanged("ProteinFromModel");
                }
            }
        }
        #endregion // OpenFileCommand

        #region SearchPubMedCommand
        #endregion // SearchPubMedCommand
        /// <summary>
        /// Returns the command which, when executed, search pubmed based on user input and retrieves
        /// hit count table
        /// </summary>
        public ICommand SearchPubMedCommand
        {
            get { return _searchPubMedCommand ?? (_searchPubMedCommand = new RelayCommand(GetCount)); }
        }

        public void GetCount()
        {
            // this is just an example for test
            UserInputFromModel.ProteinInModel = "ips";
            UserInputFromModel.OrganismInModel = "Human";
            UserInputFromModel.KeywordInModel = "cell";

            _count = PubMedSearch.GetCount(UserInputFromModel.ProteinInModel, UserInputFromModel.OrganismInModel,
                UserInputFromModel.KeywordInModel);
            CountListWithProteins.Add(new HitCountTable(_count, ProteinFromModel));
        }
        #endregion // Commands

        #region Properties
        
            #region ProteinFromModel
            public string ProteinFromModel
            {
                get
                {
                    return UserInputFromModel.ProteinInModel; }
                set
                {
                    UserInputFromModel.ProteinInModel = value;
                    OnPropertyChanged("ProteinFromModel");
                }
            }
            #endregion // ProteinFromModel

            #region OrganismFromModel
            public string OrganismFromModel
            {
                get
                { return UserInputFromModel.OrganismInModel; }
                set
                {
                    UserInputFromModel.OrganismInModel = value;
                    OnPropertyChanged("OrganismFromModel");
                }
            }
            #endregion // OrganismFromModel

            #region KeywordFromModel
            public string KeywordFromModel
            {
                get
                { return UserInputFromModel.KeywordInModel; }
                set
                {
                    UserInputFromModel.KeywordInModel = value;
                    OnPropertyChanged("KeywordFromModel");
                }
            }
            #endregion // KeywordFromModel
        #endregion

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
