﻿
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
<<<<<<< HEAD
using System.Windows;

using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using HighThroughputDataRetrievalBackend.IO;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
=======
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
<<<<<<< HEAD
=======
using Microsoft.Win32;
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae
>>>>>>> a3be286fe852bb8bb7a1312965571f7fb33f2dbd


namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        # region Fields

        public UserInput UserInputFromModel;
        public NcbiDataRetrieval PubMedSearch { get; set; }
        public List<int> CountList { get; set; }
        public IEnumerable<string> ProteinList { get; set; }
<<<<<<< HEAD
        public ObservableCollection<HitCountTable> CountProteinTable { get; set; }
        public int SelectedIndex { get; set; }
        public List<string> IDList { get; set; } 
        //
        //
        private ObservableCollection<ArticleTableInfo> _resultTable;
=======
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        public int SelectIndex { get; set; }

        
        #region Fields of paging 
        private int _start = 0;
        private const int ItemCount = 5;
        private int _totalItems = 0;
        private ICommand _firstCommand;
        private ICommand _previousCommand;
        private ICommand _nextCommand;
        private ICommand _lastCommand;


        public int Start { get { return _start + 1; } }

        /// <summary>
        /// Gets the index of the last article
        /// </summary>
        public int End { get { return _start + ItemCount < _totalItems ? _start + ItemCount : _totalItems; } }

        /// <summary>
        /// The number of total article.
        /// </summary>
        public int TotalItems { get { return _totalItems; } }

// ReSharper disable once InconsistentNaming
        public ObservableCollection<ArticleTableInfo> _ResultTable;
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae

// ReSharper disable once InconsistentNaming
        public ObservableCollection<ArticleTableInfo> _FillArticleTableInfo;

        #endregion
        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;
        RelayCommand _openHelpDocumentCommand;
        RelayCommand _retrieveArticleInfoCommand;
        RelayCommand _exportCommand;
        private RelayCommand _ArticleInfotableClick;
        #endregion // Fields
<<<<<<< HEAD

        #region Constructor

        public MainWindowViewModel()
        {
            UserInputFromModel = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountProteinTable = new ObservableCollection<HitCountTable>();
            CountList = new List<int>();
            IDList = new List<string>();

            _progressDialog = new ProgressDialog();
            _progressDialog.DoWork += _progressDialog_DoWork;

            //this.RetrieveArticleInfo();

        }

        #endregion // Constructor


        public ObservableCollection<ArticleTableInfo> ResultTable
        {
            get { return _resultTable; }
            set
            {
                _resultTable = value;
                OnPropertyChanged("ProteinFromModel");
            }
        }

        public void RetrieveArticleInfo()     
        {
            _resultTable = new ObservableCollection<ArticleTableInfo>();
            int count = CountProteinTable[SelectedIndex].CountInHitCountTable;
            string protein = CountProteinTable[SelectedIndex].ProteinInHitCountTable;
            string name = protein + OrganismFromModel + KeywordFromModel;
            
            DataTable queryDataTable = PubMedSearch.QueryDataTable;
            DataTable queryArticleDataTable = PubMedSearch.QueryArticlesDataTable;

            // get queryID from queryDataTable using name
            string queryID = "";
            foreach (DataRow row in queryDataTable.Rows)
            {
                if (row["Name"].ToString() == name)
                {
                    queryID = row["QueryID"].ToString();
                    break;
                }
            }

            // get PMIDs from queryArticleDataTable using queryID
            DataRow[] queryArticleDataRows = queryArticleDataTable.Select("QueryID = " + queryID);
            foreach (DataRow row in queryArticleDataRows)
            {
                IDList.Add(row["PMID"].ToString());
            }

            // retrieve article information with the count and the ID list
            // after call this method, the datatables are filled with article information
            DataTable articleDataTable = PubMedSearch.GetArticleInfomation(count, IDList);

            // get authorListDataTable
            DataTable authorListDataTable = PubMedSearch.AuthorListDataTable;

            // get authroDataTable
            DataTable authorsDataTable = PubMedSearch.AuthorsDataTable;

            // get journalReleaseDataTable 
            DataTable journalReleaseDataTable = PubMedSearch.JournalReleaseDataTable;

            // get journalDataTable
            DataTable journalDataTable = PubMedSearch.JournalDataTable;

           //DataSet temp = PubMedSearch.GetDataSet();
           //temp.WriteXml("check.xml");
          
            // get article, author, and journal information
            foreach (DataRow row in articleDataTable.Rows)
            {
                // get title, url, and published date
                string articleTitle = row["Title"].ToString();
                string url = row["URL"].ToString();
                string pubDate = row["PubDate"].ToString();

                // get authors from authorListDataTable using article's PMID
                string pmid = string.Format("{0}{1}{2}", "'", row["PMID"].ToString(), "'");
                DataRow[] authorListDataRows = authorListDataTable.Select("PMID =" + pmid);

                // most authors' name is last name and first name, 
                // but some author has suffix such as Junior
                // and also some author has just only have collective name
                // only print first and last author on the grid.

                // get first author 
                DataRow authorFirstDataRow = authorsDataTable.Rows.Find(authorListDataRows[0]["AuthorID"].ToString());
                string authorFirst = authorFirstDataRow["Suffix"].ToString();

                if (authorFirst != "") // author has suffix, so put " "
                {
                    authorFirst += " ";
                }
                authorFirst += (authorFirstDataRow["LastName"].ToString() + ", "
                                + authorFirstDataRow["ForeName"].ToString());

                // author name is not last name and first name, CollectiveName
                if (authorFirst == "") 
                {
                    authorFirst = authorFirstDataRow["CollectiveName"].ToString();
                }


                // get last author
                int lastAuthorIndex = authorListDataRows.Count() - 1;
                DataRow authorLastDataRow = authorsDataTable.Rows.Find(authorListDataRows[lastAuthorIndex]["AuthorID"].ToString());
                string authorLast = authorLastDataRow["Suffix"].ToString();

                if (authorLast != "") // author has suffix, so put " "
                {
                    authorLast += " ";
                }

                authorLast += (authorLastDataRow["LastName"].ToString() + ", "
                               + authorLastDataRow["ForeName"].ToString());

                if (authorLast == "") // author has collective name, not last name and first name.
                {
                    authorLast = authorLastDataRow["CollectiveName"].ToString();
                }

                // combine the first author and the last author
                string authors = authorFirst + " ... " + authorLast;


                string journalRelease = row["JournalRelease"].ToString();
                DataRow journalReleaseDataRow = journalReleaseDataTable.Rows.Find(journalRelease);
                DataRow journalDataRow = journalDataTable.Rows.Find(journalReleaseDataRow["JournalID"].ToString());
                string journalTitle = journalDataRow["Title"].ToString();


                _resultTable.Add(new ArticleTableInfo() { ArticleTitle = articleTitle, Author = authors, Year = pubDate, Journal = journalTitle, Url = url });
            }

=======
         
        public ICommand ArticleInfoTableClick {
            get
            {
                return _ArticleInfotableClick ?? (_ArticleInfotableClick = new RelayCommand(Article_Info_Table_Click));
            }
        }

        public void Article_Info_Table_Click()
        {
            if (MessageBox.Show("Go to PubMed ?", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //Hyperlink link = new Hyperlink(FillArticleTableInfo[SelectIndex].Url.ToString());
               // Uri uri = new Uri(FillArticleTableInfo[SelectIndex].Url.ToString());

                Process.Start(FillArticleTableInfo[SelectIndex].Url);
            }
        }
       

        public ObservableCollection<ArticleTableInfo> FillArticleTableInfo
        {
            get { return _FillArticleTableInfo; }
            set
            {
                _FillArticleTableInfo = value;
                OnPropertyChanged("FillArticleTableInfo");
            }
        }
        public void RetrievalArticleInfo()
        {
<<<<<<< HEAD
            _ResultTable = new ObservableCollection<ArticleTableInfo>();
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
            article.Rows.Add("abcd ", 3, "http://www.ncbi.nlm.nih.gov/pubmed/");
            article.Rows.Add("abcd1", 4, "http://www.ncbi.nlm.nih.gov/pubmed/");
            article.Rows.Add("lasdasd ", 5, "http://www.ncbi.nlm.nih.gov/pubmed/");
            article.Rows.Add("Last", 6, "http://www.ncbi.nlm.nih.gov/pubmed/");
           
            DataTable author = new DataTable("Author");
            author.Columns.Add("Author", typeof(string));
            author.Columns.Add("PMID", typeof(int));
            author.Rows.Add("Lyukmanova EN,…Tsetlin VI", 1);
            author.Rows.Add("Shulepko MA, … Kirpichnikov MP", 2);
            author.Rows.Add("Lyukmanova EN,…Tsetlin VI", 3);
            author.Rows.Add("Shulepko MA, … Kirpichnikov MP", 4);
            author.Rows.Add("Lyukmanova EN,…Tsetlin VI", 5);
            author.Rows.Add("Shulepko MA, … Kirpichnikov MP", 6);

            DataTable journal = new DataTable("Jornal");
            journal.Columns.Add("Jornal", typeof(string));
            journal.Columns.Add("Year", typeof(int));
            journal.Columns.Add("PMID", typeof(int));
            journal.Rows.Add("J Biol Chem", 2013, 1);
            journal.Rows.Add("Bioorg Khim", 2011, 2);
            journal.Rows.Add("J Biol Chem", 2013, 3);
            journal.Rows.Add("Bioorg Khim", 2011, 4);
            journal.Rows.Add("J Biol Chem", 2013, 5);
            journal.Rows.Add("Bioorg Khim", 2011, 6);

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
                _ResultTable.Add(new ArticleTableInfo { ArticleTitle = myArticle[i], Author = myAuthor[i], Year = myYear[i], Journal = myJournal[i], Url = myUrl[i] });
            }
            _totalItems = _ResultTable.Count;
=======
            
            
>>>>>>> a3be286fe852bb8bb7a1312965571f7fb33f2dbd
        }
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae

            //MessageBox.Show("Retrieving article info done!");

            //for (int i = 0; i < article.Rows.Count; i++)
            //{
            //    _resultTable.Add(new ArticleTableInfo() { ArticleTitle = myArticle[i], Author = myAuthor[i], Year = myYear[i], Journal = myJournal[i], Url = myUrl[i] });
            //}
            //string[] myArticle = new string[15];
            //string[] myAuthor = new string[15];
            //int[] myYear = new int[15];
            //string[] myJournal = new string[15];
            //string[] myUrl = new string[15];
            ////example table in dataset
            //DataTable article = new DataTable("Article");
            //article.Columns.Add("Title", typeof(string));
            //article.Columns.Add("PMID", typeof(int));
            //article.Columns.Add("URL", typeof(string));
            //article.Rows.Add("Water-soluble LYNX1 residues important for interaction with muscle-type and/or neuronal nicotinic receptors", 1, "http://www.ncbi.nlm.nih.gov/pubmed/");
            //article.Rows.Add("[Bacterial expression of water-soluble domain of Lynx1, endogenic neuromodulator of humannicotinic acetylcholine receptors]", 2, "http://www.ncbi.nlm.nih.gov/pubmed/");

            //DataTable author = new DataTable("Author");
            //author.Columns.Add("Author", typeof(string));
            //author.Columns.Add("PMID", typeof(int));
            //author.Rows.Add("Lyukmanova EN,…Tsetlin VI", 1);
            //author.Rows.Add("Shulepko MA, … Kirpichnikov MP", 2);

            //DataTable journal = new DataTable("Jornal");
            //journal.Columns.Add("Jornal", typeof(string));
            //journal.Columns.Add("Year", typeof(int));
            //journal.Columns.Add("PMID", typeof(int));
            //journal.Rows.Add("J Biol Chem", 2013, 1);
            //journal.Rows.Add("Bioorg Khim", 2011, 2);

            ////get myArticle
            //for (int i = 0; i < article.Rows.Count; i++)
            //{
            //    myArticle[i] = string.Format(article.Rows[i].ItemArray[0].ToString());
            //    myUrl[i] = string.Format(article.Rows[i].ItemArray[2].ToString());

            //}
            ////get myAuthor

            //for (int i = 0; i < article.Rows.Count; i++)
            //{
            //    for (int j = 0; j < author.Rows.Count; j++)
            //    {
            //        if ((int)article.Rows[i].ItemArray[1] == (int)author.Rows[j].ItemArray[1])
            //        {
            //            myAuthor[i] = (string)author.Rows[j].ItemArray[0];
            //        }
            //    }
            //}
            ////get myJournal and my Year
            //for (int i = 0; i < article.Rows.Count; i++)
            //{
            //    for (int j = 0; j < journal.Rows.Count; j++)
            //    {
            //        if ((int)article.Rows[i].ItemArray[1] == (int)journal.Rows[j].ItemArray[2])
            //        {
            //            myJournal[i] = (string)journal.Rows[j].ItemArray[0];
            //            myYear[i] = (int)journal.Rows[j].ItemArray[1];
            //        }
            //    }
            //}

<<<<<<< HEAD

=======
        public MainWindowViewModel()      
        {
            UserInputFromModel  = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountListWithProteins = new ObservableCollection<HitCountTable>();
            CountList = new List<int>();
<<<<<<< HEAD
            RetrievalArticleInfo();
            Refresh();
            SqliteInputOutput.Create_database("../../Document/HTDR_Database.db3");

            _progressDialog = new ProgressDialog();
            _progressDialog.DoWork += _progressDialog_DoWork;

=======
            this.RetrievalArticleInfo();
            Refresh();

            LoadDataGrid();
            _progressDialog = new ProgressDialog();
            _progressDialog.DoWork += _progressDialog_DoWork;

           
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae
>>>>>>> a3be286fe852bb8bb7a1312965571f7fb33f2dbd
        }



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

        public ICommand RetrieveArticleInfoCommand
        {
            get { return _retrieveArticleInfoCommand ?? (_retrieveArticleInfoCommand = new RelayCommand(RetrieveArticleInfo)); }
        }

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
                //var newString = string.Join(" ", Regex.Split(oldString, @"(?:\r\n|\n|\r)"));
                // parse proteins string into a list
                //ProteinList = Regex.Split(ProteinFromModel, @"(?:\r\n|\n|\r)");
                ProteinFromModel=Regex.Replace(ProteinFromModel, @"[\r\n]+", "\n", RegexOptions.Multiline).Trim();
                ProteinList=Regex.Split(ProteinFromModel, "\n");

                //ShowProgressDialog();

                foreach (string protein in ProteinList)
                {
                    int count = PubMedSearch.GetCount(protein, OrganismFromModel, KeywordFromModel);
                    CountList.Add(count);
                    CountProteinTable.Add(new HitCountTable(count, protein));
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

            #region buttion_export
            public ICommand ExpordCommand
            {
                get { return _exportCommand ?? (_exportCommand = new RelayCommand(Expord)); }
            }
            public void Expord()
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
                            while (count < _ResultTable.Count)
                            {
                                writer.WriteLine(_ResultTable[count].ArticleTitle);
                                writer.WriteLine(_ResultTable[count].Author);
                                writer.WriteLine(_ResultTable[count].Journal);
                                writer.WriteLine(_ResultTable[count].Year);
                                writer.WriteLine(_ResultTable[count].Url);
                                count++;
                            }
                            writer.Close();
                        }
                        stream.Close();
                    }
                }

            }
            #endregion


            #region paging
            public ICommand FirstCommand
            {
                get
                {
                    return _firstCommand ?? (_firstCommand = new RelayCommand_
                        (
                        param =>
                        {
                            _start = 0;
                            Refresh();
                        },
                        param => _start - ItemCount >= 0));
                }
            }

            /// <summary>
            /// Gets the command for moving to the previous page
            /// </summary>
            public ICommand PreviousCommand
            {
                get
                {
                    return _previousCommand ?? (_previousCommand = new RelayCommand_
                        (
                        param =>
                        {
                            _start -= ItemCount;
                            Refresh();
                        },
                        param => _start - ItemCount >= 0));
                }
            }

            /// <summary>
            /// Gets the command for moving to the next page 
            /// </summary>
            public ICommand NextCommand
            {
                get
                {
                    return _nextCommand ?? (_nextCommand = new RelayCommand_
                        (
                        param =>
                        {
                            _start += ItemCount;
                            Refresh();
                        },
                        param => { return _start + ItemCount < _totalItems ? true : false; }
                        ));
                }
            }

            /// <summary>
            /// Gets the command for moving to the last page
            /// </summary>
            public ICommand LastCommand
            {
                get
                {
                    if (_lastCommand == null)
                    {
                        _lastCommand = new RelayCommand_
                        (
                            param =>
                            {
                                _start = (_totalItems / ItemCount - 1) * ItemCount;
                                _start += _totalItems % ItemCount == 0 ? 0 : ItemCount;
                                Refresh();
                            },
                            param =>
                            {
                                return _start + ItemCount < _totalItems ? true : false;
                            }
                        );
                    }

                    return _lastCommand;
                }
            }

            /// <summary>
            /// Refreshes the list article info
            /// </summary>
            private void Refresh()
            {
                FillPage(_start);
                OnPropertyChanged("Start");
                OnPropertyChanged("End");
                OnPropertyChanged("TotalItems");
                OnPropertyChanged("FillArticleTableInfo");
            }

            public void FillPage(int startNum)
            {
                this._FillArticleTableInfo = new ObservableCollection<ArticleTableInfo>();
                for (int i = startNum; i < startNum + ItemCount && i < _totalItems; i++)

                {
                  _FillArticleTableInfo.Add(_ResultTable[i]);
                }
            }

            /// <summary>
            /// Notifies subscribers of changed properties.
            /// </summary>
            /// <param name="propertyName">Name of the changed property.</param>
           
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
<<<<<<< HEAD
=======

<<<<<<< HEAD
    public class DataGrid
    {
        public string ArticleTitle { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Journal { get; set; }
    }

   
=======
    
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae

>>>>>>> a3be286fe852bb8bb7a1312965571f7fb33f2dbd
}
