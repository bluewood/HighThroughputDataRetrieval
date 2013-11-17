using System;
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
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using HighThroughputDataRetrievalBackend.IO;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        public UserInput UserInputFromModel;

        private ProgressDialog _ProgressDialog = new ProgressDialog()
        {
            Text = "Retrieving publication number from PubMed...",
            Description = "Processing...",
            ShowTimeRemaining = true,
        };

        // Hyesun added for GetCount
        public NcbiDataRetrieval PubMedSearch { get; set; }
        public List<int> CountList { get; set; }
        public IEnumerable<string> ProteinList { get; set; }
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        //
        //
        public ObservableCollection<ArticleTableInfo> _ResultTable;

        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;
        RelayCommand _openHelpDocumentCommand;
        RelayCommand _retrieveArticleInfoCommand;
        RelayCommand _ExportCommand;
    
        #endregion // Fields

        public ObservableCollection<ArticleTableInfo> ResultTable
        {
            get { return _ResultTable; }
            set
            {
                _ResultTable = value;
                OnPropertyChanged("ProteinFromModel");
            }
        }

        public void RetrieveArticleInfo()
        {
            this._ResultTable = new ObservableCollection<ArticleTableInfo>();
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

            int articleCount = article.Rows.Count;

            // Run progress dialog on background worker, which is not UI thread
            ShowProgressDialog();

            //get myArticle
            for (int i = 0; i < article.Rows.Count; i++)
            {
                myArticle[i] = string.Format(article.Rows[i].ItemArray[0].ToString());
                myUrl[i] = string.Format(article.Rows[i].ItemArray[2].ToString());
                CurrentProgress = i*100/articleCount;
                OnPropertyChanged("CurrentProgress");
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
                _ResultTable.Add(new ArticleTableInfo() { ArticleTitle = myArticle[i], Author = myAuthor[i], Year = myYear[i], Journal = myJournal[i], Url = myUrl[i] });
            }
            
            SwitchArticleAbstractTab = true; // Switch to ArticleAbstractTab by setting ArticleAbstractTab IsSelected = true
        }

        #region Constructor

       

        public MainWindowViewModel()
        {
           // MainWindow window = new MainWindow();
           // window.Closing += MyWindow_Closing;// Subsribes to close window event
            
            SqliteInputOutput.Create_database("../../../HighThroughputDataRetrievalBackend/Model/database.db3");
            UserInputFromModel  = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountListWithProteins = new ObservableCollection<HitCountTable>();
            CountList = new List<int>();

           _ProgressDialog.DoWork += _ProgressDialog_DoWork;

            //RetrievalArticleInfo();
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

        #region BrowsePreviousResultCommand
        /// <summary>
        /// Returns the command which, when executed, load query input from database into textboxes

        // </summary>
        public ICommand BrowsePreviousResultCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand(LoadPreviousQuery)); }
        }

        void LoadPreviousQuery()
        {
            
        }
        #endregion // BrowsePreviousResultCommand

        #region SearchPubMedCommand
        
        /// <summary>
        /// Returns the command which, when executed, search pubmed based on user input and retrieves
        /// hit count table
        /// </summary>
        public ICommand SearchPubMedCommand
        {
            get { return _searchPubMedCommand ?? (_searchPubMedCommand = new RelayCommand(GetCount)); }
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


        public ICommand RetrieveArticleInfoCommand
        {
            get { return _retrieveArticleInfoCommand ?? (_retrieveArticleInfoCommand = new RelayCommand(RetrieveArticleInfo)); }
        }
        #endregion // Commands

        #region Methods
        private void ShowProgressDialog()
        {
            if (_ProgressDialog.IsBusy)
                MessageBox.Show("The progress dialog is already displayed.");
            else
                _ProgressDialog.Show(); // Show a modeless dialog
        }

        private void _ProgressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
              while(CurrentProgress < 100)
              {
               Thread.Sleep(500);
                // Periodically check CancellationPending and abort the operation if required.
                if (_ProgressDialog.CancellationPending)
                    return;
                // The time will automatically be calculated based on the frequency of the calls to ReportProgress.
                _ProgressDialog.ReportProgress(CurrentProgress, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Processing: {0}%", CurrentProgress));
            }
        }

        public void GetCount()
        {
            if (UserInputFromModel.ProteinInModel == null)
            {
                MessageBox.Show("Protein field required");
            }
            else
            {
                // Replace the repetition of occurence of "\r\n" with "\n" in ProteinFromModel to get ready for parsing,
                // so a blank line in input textbox will not be counted
                ProteinFromModel = Regex.Replace(ProteinFromModel, @"[\r\n]+", "\n", RegexOptions.Multiline).Trim();
               
                // Parse string ProteinFromModel into ProteinList by the occurence of "\n"
                ProteinList = Regex.Split(ProteinFromModel, "\n");

                int proteinCounter = 0;
                int totalProtein = ProteinList.Count();

                // Run progress dialog on background worker, which is not UI thread
                ShowProgressDialog();
                
                foreach (string protein in ProteinList)
                {
                    int count = PubMedSearch.GetCount(protein, OrganismFromModel, KeywordFromModel);
                    CountList.Add(count);
                    CountListWithProteins.Add(new HitCountTable(count, protein));
                    proteinCounter++;
                    CurrentProgress = proteinCounter*100/totalProtein;
                    OnPropertyChanged("CurrentProgress");
                }
                
                SwitchHitCountTableTab = true; // Switch to HitCountTableTab by setting HitCountTableTab IsSelected = true
            }
        }

        private int _currentProgress;
        public int CurrentProgress
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged("CurrentProgress");
            }
        }

        public void OpenHelpDocument()
        {
            var helpDocumentWindow = new HelpDocumentView();
            helpDocumentWindow.Show();
        }
        #endregion  // Methods
        
        #region Properties

        #region SwitchHitCountTableTab
        /// <summary>
        /// SwitchHitCountTableTab is a property binding to HitCountTableTab IsSelected. By setting SwitchHitCountTableTab=true,
        /// it will select HitCountTableTab programmatically.
        /// </summary>
        private bool _switchHitCountTableTab;

        public bool SwitchHitCountTableTab
        {
            get { return _switchHitCountTableTab; }
            set
            {
                _switchHitCountTableTab = value;
                OnPropertyChanged("SwitchHitCountTableTab");
            }
        }
        #endregion // SwitchHitCountTableTab


        #region SwitchArticleAbstractTab

        /// <summary>
        /// SwitchArticleAbstractTab is a property binding to ArticleAbstractTab IsSelected. By setting SwitchArticleAbstractTab=true,
        /// it will select ArticleAbstractTab programmatically.
        /// </summary>
        private bool _switchArticleAbstractTab;

        public bool SwitchArticleAbstractTab
        {
            get { return _switchArticleAbstractTab; }
            set
            {
                _switchArticleAbstractTab = value;
                OnPropertyChanged("SwitchArticleAbstractTab");
            }
        }
        #endregion // SwitchHitCountTableTab

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

            #region buttion_export
            public ICommand ExportCommand
            {
                get { return _ExportCommand ?? (_ExportCommand = new RelayCommand(Export)); }
            }
            public void Export()
            {
                int count = 0;
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    DefaultExt = "txt",
                    Filter = "TXT Files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 1
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            while (count < this.ResultTable.Count)
                            {
                                writer.WriteLine(this.ResultTable[count].ArticleTitle);
                                writer.WriteLine(this.ResultTable[count].Author);
                                writer.WriteLine(this.ResultTable[count].Journal);
                                writer.WriteLine(this.ResultTable[count].Year);
                                writer.WriteLine(this.ResultTable[count].Url);
                                count++;
                            }
                            writer.Close();
                        }
                        stream.Close();
                    }
                }

            }
            #endregion

         public void MyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
             e.Cancel = true;

            MessageBoxResult result = MessageBox.Show("Save query into database?", "My Title", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) // Save query into database
            {
                //get dataset
                //copy dataset to database
                DataSet myDataSet = PubMedSearch.GetDataSet();


                bool success = SqliteInputOutput.CopydatasetToDatabase("../../../HighThroughputDataRetrievalBackend/Model/database.db3", myDataSet);

            }
            else if (result == MessageBoxResult.No) // Close the application if the user would not like to save query, 
            {
                e.Cancel = false;
            }
        }


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
