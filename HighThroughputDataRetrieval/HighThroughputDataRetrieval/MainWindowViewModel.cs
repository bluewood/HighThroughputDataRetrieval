using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        public UserInput UserInputFromModel;
        //
        // Hyesun added for GetCount
        public NcbiDataRetrieval PubMedSearch { get; set; }
        public List<int> CountList { get; set; }
        public IEnumerable<string> ProteinList { get; set; }
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        //
        //
        public ObservableCollection<DataGrid> _ResultTable;

        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;
        RelayCommand _openHelpDocumentCommand;
        RelayCommand _click;

        #endregion // Fields

        public ObservableCollection<DataGrid> ResultTable
        {
            get { return _ResultTable; }
            set
            {
                _ResultTable = value;
                OnPropertyChanged("ProteinFromModel");
            }
        }

        public void LoadDataGrid()
        {
            this._ResultTable = new ObservableCollection<DataGrid>();
            string[] myArticle = new string[15];
            string[] myAuthor = new string[15];
            int[] myYear = new int[15];
            string[] myJournal = new string[15];
            string[] myUrl = new string[15];
            //example table in dataset
            DataTable article = new DataTable("Article");
            article.Columns.Add("Title", typeof(string));
            article.Columns.Add("PMID", typeof(int));
            article.Columns.Add("URL", typeof(string));
            article.Rows.Add("Water-soluble LYNX1 residues important for interaction with muscle-type and/or neuronal nicotinic receptors", 1, "http://www.ncbi.nlm.nih.gov/pubmed/");
            article.Rows.Add("[Bacterial expression of water-soluble domain of Lynx1, endogenic neuromodulator of humannicotinic acetylcholine receptors]", 2, "http://www.ncbi.nlm.nih.gov/pubmed/");

            DataTable author = new DataTable("Author");
            author.Columns.Add("Author", typeof(string));
            author.Columns.Add("PMID", typeof(int));
            author.Rows.Add("Lyukmanova EN,…Tsetlin VI", 1);
            author.Rows.Add("Shulepko MA, … Kirpichnikov MP", 2);

            DataTable journal = new DataTable("Jornal");
            journal.Columns.Add("Jornal", typeof(string));
            journal.Columns.Add("Year", typeof(int));
            journal.Columns.Add("PMID", typeof(int));
            journal.Rows.Add("J Biol Chem", 2013, 1);
            journal.Rows.Add("Bioorg Khim", 2011, 2);

            //get myArticle
            for (int i = 0; i < article.Rows.Count; i++)
            {
                myArticle[i] = string.Format(article.Rows[i].ItemArray[0].ToString());
                myUrl[i] = string.Format(article.Rows[i].ItemArray[2].ToString());

            }
            //get myAuthor

            for (int i = 0; i < article.Rows.Count; i++)
            {
                for (int j = 0; j < author.Rows.Count; j++)
                {
                    if ((int)article.Rows[i].ItemArray[1] == (int)author.Rows[j].ItemArray[1])
                    {
                        myAuthor[i] = (string)author.Rows[j].ItemArray[0];
                    }
                }
            }
            //get myJournal and my Year
            for (int i = 0; i < article.Rows.Count; i++)
            {
                for (int j = 0; j < journal.Rows.Count; j++)
                {
                    if ((int)article.Rows[i].ItemArray[1] == (int)journal.Rows[j].ItemArray[2])
                    {
                        myJournal[i] = (string)journal.Rows[j].ItemArray[0];
                        myYear[i] = (int)journal.Rows[j].ItemArray[1];
                    }
                }
            }
            for (int i = 0; i < article.Rows.Count; i++)
            {
                _ResultTable.Add(new DataGrid() { ArticleTitle = myArticle[i], Author = myAuthor[i], Year = myYear[i], Journal = myJournal[i], Url = myUrl[i] });
            }

        }

        public ICommand click
        {
            get { return _click ?? (_click = new RelayCommand(ClickOnDataGrid)); }

        }

        public void ClickOnDataGrid()
        {
            if (MessageBox.Show("Go to PubMed ?", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RoutedEventArgs e = new RoutedEventArgs();
                Hyperlink link = (Hyperlink)e.OriginalSource;
                Process.Start(link.NavigateUri.AbsoluteUri);
            }
        }

        #region Constructor

        public MainWindowViewModel()      
        {
            UserInputFromModel  = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountListWithProteins = new ObservableCollection<HitCountTable>();
            CountList = new List<int>();

            LoadDataGrid();
            _progressDialog = new ProgressDialog();
            _progressDialog.DoWork += _progressDialog_DoWork;
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
        
        /// <summary>
        /// Returns the command which, when executed, search pubmed based on user input and retrieves
        /// hit count table
        /// </summary>
        public ICommand SearchPubMedCommand
        {
            get { return _searchPubMedCommand ?? (_searchPubMedCommand = new RelayCommand(GetCount)); }
        }
        /// <summary>
        /// Nan: need revision, change to multi-threading 
        /// current progress bar is on single thread, which is time consuming
        /// </summary>
        private ProgressDialog _progressDialog = new ProgressDialog()
        {
            Text = "Retrieving count number from PubMed...",
            Description = "Processing...",
            ShowTimeRemaining = true,
        };

        public void ShowProgressDialog()
        {
            if (_progressDialog.IsBusy)
                MessageBox.Show("The progress dialog is already displayed.");
            else
                _progressDialog.Show(); // Show a modeless dialog
        }

        private void _progressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            int x = 0;
            // Implement the operation that the progress bar is showing progress of here, same as you would do with a background worker.
            foreach (string protein in ProteinList)
            {
                Thread.Sleep(500);
                // Periodically check CancellationPending and abort the operation if required.
                if (_progressDialog.CancellationPending)
                    return;

                //int count = PubMedSearch.GetCount(protein, OrganismFromModel, KeywordFromModel);
                //CountList.Add(count);
                //CountListWithProteins.Add(new HitCountTable(count, protein));

                x++;
                // ReportProgress can also modify the main text and description; pass null to leave them unchanged.
                // If _progressDialog.ShowTimeRemaining is set to true, the time will automatically be calculated based on
                // the frequency of the calls to ReportProgress.
                _progressDialog.ReportProgress(x, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Processing: {0}%", (int)x / ProteinList.Count()));
            }
        }
        #endregion // SearchPubMedCommand

        #region OpenHelpDocumentCommand
        public ICommand OpenHelpDocumentCommand
        {
            get
            {
                return _openHelpDocumentCommand ?? (_openHelpDocumentCommand = new RelayCommand(OpenHelpDocument));
            }
        }
        #endregion

        #endregion // Commands

        #region Methods
        public void GetCount()
        {
            if (UserInputFromModel.ProteinInModel == null)
            {
                MessageBox.Show("Protein field required");
            }
            else
            {
                // parse proteins string into a list
                ProteinList = Regex.Split(ProteinFromModel, "\n");

                ShowProgressDialog();

                foreach (string protein in ProteinList)
                {
                    int count = PubMedSearch.GetCount(protein, OrganismFromModel, KeywordFromModel);
                    CountList.Add(count);
                    CountListWithProteins.Add(new HitCountTable(count, protein));
                }
               
                SwitchTab = true; // Switch to HitCountTableTab by setting HitCountTableTab IsSelected = true
            }
        }

        public void OpenHelpDocument()
        {

            var helpDocumentWindow = new HelpDocumentView();
            helpDocumentWindow.Show();

        }
        #endregion
        
       

        #region Properties

        #region SwitchTab
        /// <summary>
        /// SwitchTab is a property binding to HitCountTableTab IsSelected. By setting SwitchTab=true,
        /// it will select HitCountTableTab programmatically.
        /// </summary>
        private bool _switchTab;

        public bool SwitchTab
        {
            get { return _switchTab; }
            set
            {
                _switchTab = value;
                OnPropertyChanged("SwitchTab");
            }
        }
        #endregion

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
    public class DataGrid
    {
        public string ArticleTitle { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Journal { get; set; }
    }
}
