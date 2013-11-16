
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
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using HighThroughputDataRetrievalBackend.IO;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        # region Fields

        public UserInput UserInputFromModel;
        public NcbiDataRetrieval PubMedSearch { get; set; }
        public List<int> CountList { get; set; }
        public IEnumerable<string> ProteinList { get; set; }
        public ObservableCollection<HitCountTable> CountProteinTable { get; set; }
        public int SelectedIndex { get; set; } // in HitCountTable
        public int SelectIndex { get; set; }   // in ArticleInfoTable
        public List<string> IDList { get; set; } 
        private ObservableCollection<ArticleTableInfo> _resultTable;
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        

        
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

// ReSharper disable once InconsistentNaming
        public ObservableCollection<ArticleTableInfo> _FillArticleTableInfo;

        #endregion // fields of paging

        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;
        RelayCommand _openHelpDocumentCommand;
        RelayCommand _retrieveArticleInfoCommand;
        RelayCommand _exportCommand;
        RelayCommand _articleInfotableClick;


        #endregion // Fields

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


        #region ArticleInfoTableClick
        public ICommand ArticleInfoTableClickCommand {
            get
            {
                return _articleInfotableClick ?? (_articleInfotableClick = new RelayCommand(ArticleInfoTableClick));
            }
        }

        public void ArticleInfoTableClick()
        {
            if (MessageBox.Show("Go to PubMed ?", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //Hyperlink link = new Hyperlink(FillArticleTableInfo[SelectIndex].Url.ToString());
               // Uri uri = new Uri(FillArticleTableInfo[SelectIndex].Url.ToString());

                Process.Start(FillArticleTableInfo[SelectIndex].Url);
            }
        }

        #endregion //articleInfoTableClick

        public ObservableCollection<ArticleTableInfo> FillArticleTableInfo
        {
            get { return _FillArticleTableInfo; }
            set
            {
                _FillArticleTableInfo = value;
                OnPropertyChanged("FillArticleTableInfo");
            }
        }
  



       
        #region OpenFile
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
        #endregion // OpenFile


        #region SearchPubMedCommand 
        
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
            if (UserInputFromModel.ProteinInModel == null)
            {
                MessageBox.Show("Protein field required");
            }
            else
            {
                //var newString = string.Join(" ", Regex.Split(oldString, @"(?:\r\n|\n|\r)"));
                // parse proteins string into a list
                //ProteinList = Regex.Split(ProteinFromModel, @"(?:\r\n|\n|\r)");
                ProteinFromModel = Regex.Replace(ProteinFromModel, @"[\r\n]+", "\n", RegexOptions.Multiline).Trim();
                ProteinList = Regex.Split(ProteinFromModel, "\n");

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

        #endregion // search pubmed


        #region progressing bar
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
        #endregion 
    

        #region RetrieveArticleInfo

        public ICommand RetrieveArticleInfoCommand
        {
            get { return _retrieveArticleInfoCommand ?? (_retrieveArticleInfoCommand = new RelayCommand(RetrieveArticleInfo)); }
        }

        public void RetrieveArticleInfo()
        {
            _resultTable = new ObservableCollection<ArticleTableInfo>();


            int count = CountProteinTable[SelectedIndex].CountInHitCountTable;
            string protein = CountProteinTable[SelectedIndex].ProteinInHitCountTable;
            string name = protein.ToUpper() + OrganismFromModel.ToUpper() + KeywordFromModel.ToUpper();

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
            string filerExpression = string.Format("QueryID = {0}{1}{2}", "'", queryID.ToString(), "'");
            DataRow[] queryArticleDataRows = queryArticleDataTable.Select(filerExpression);
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
                string filterExpression = string.Format("{0}{1}{2}", "'", row["PMID"].ToString(), "'");
                DataRow[] authorListDataRows = authorListDataTable.Select("PMID =" + filterExpression);

                string authors = "";
                if (authorListDataRows.Count() != 0)
                {
                    DataRow firstAuthorID = authorListDataRows.First();
                    DataRow lastAuthorID = authorListDataRows.Last();
                    //DataRow authorFirst = authorListDataRows;
                    //int index = authorListDataRows.Count();
                    // most authors' name is last name and first name, 
                    // but some author has suffix such as Junior
                    // and also some author has just only have collective name
                    // only print first and last author on the grid.

                    // get first author 
                    DataRow authorFirstDataRow = authorsDataTable.Rows.Find(firstAuthorID["AuthorID"].ToString());
                    string authorFirst = authorFirstDataRow["Suffix"].ToString();

                    if (authorFirst != "") // author has suffix, so put " "
                    {
                        authorFirst += " ";
                    }
                    authorFirst += (authorFirstDataRow["LastName"].ToString() + ", "
                                    + authorFirstDataRow["ForeName"].ToString());

                    // author name is not last name and first name, CollectiveName
                    if (authorFirst == ", ")
                    {
                        authorFirst = authorFirstDataRow["CollectiveName"].ToString();
                    }


                    // get last author
                    //int lastAuthorIndex = authorListDataRows.Count() - 1;
                    DataRow authorLastDataRow =
                        authorsDataTable.Rows.Find(lastAuthorID["AuthorID"].ToString());
                    string authorLast = authorLastDataRow["Suffix"].ToString();

                    if (authorLast != "") // author has suffix, so put " "
                    {
                        authorLast += " ";
                    }

                    authorLast += (authorLastDataRow["LastName"].ToString() + ", "
                                   + authorLastDataRow["ForeName"].ToString());

                    if (authorLast == ", ") // author has collective name, not last name and first name.
                    {
                        authorLast = authorLastDataRow["CollectiveName"].ToString();
                    }

                    
                    // combine the first author and the last author
                    authors = authorFirst + " ... " + authorLast;

                }
                           

                // get journal title
                string journalRelease = row["JournalRelease"].ToString();

                string journalTitle = "";
                if (journalRelease != "")
                {
                    DataRow journalReleaseDataRow = journalReleaseDataTable.Rows.Find(journalRelease);
                    DataRow journalDataRow = journalDataTable.Rows.Find(journalReleaseDataRow["JournalID"].ToString());
                    journalTitle = journalDataRow["Title"].ToString();
                }
                    

                // put in window
                _resultTable.Add(new ArticleTableInfo()
                {
                    ArticleTitle = articleTitle,
                    Author = authors,
                    Year = pubDate,
                    Journal = journalTitle,
                    Url = url
                });

            } // end of foreach

            string isdone = "hahaha";
        }
        #endregion // RetrieveArticleInfo
     

        #region OpenHelpDocument
        public ICommand OpenHelpDocumentCommand
        {
            get
            {
                return _openHelpDocumentCommand ?? (_openHelpDocumentCommand = new RelayCommand(OpenHelpDocument));
            }
        }
        
        public void OpenHelpDocument()
        {

            var helpDocumentWindow = new HelpDocumentView();
            helpDocumentWindow.Show();

        }
        #endregion // open help document


        #region buttion_export

        public ICommand ExportCommand
        {
            get { return _exportCommand ?? (_exportCommand = new RelayCommand(Export)); }
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
        #endregion // button export


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
           
        #endregion // paing


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion // INotifyPropertyChanged Members


        #region Properties (protein, organism, keword)

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
                return UserInputFromModel.ProteinInModel;
            }
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

        #endregion // properties
    }
   
   
}
