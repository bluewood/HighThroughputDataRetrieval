using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using HighThroughputDataRetrievalBackend.Model;
using HighThroughputDataRetrievalBackend.Util;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Microsoft.Win32;

namespace HighThroughputDataRetrieval
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        # region Fields

        public UserInput UserInputFromModel;
        //
        // Hyesun added for GetCount
        public NcbiDataRetrieval PubMedSearch { get; set; }
        public List<int> CountList { get; set; }
        public IEnumerable<string> ProteinList { get; set; }
        public ObservableCollection<HitCountTable> CountListWithProteins { get; set; }
        //

        public int SelectIndex { get; set; }

        #region Fields of paging 
        private int start = 0;
        private int itemCount =5 ;
        private int totalItems = 0;
        private ICommand firstCommand;
        private ICommand previousCommand;
        private ICommand nextCommand;
        private ICommand lastCommand;


        public int Start { get { return start + 1; } }

        /// <summary>
        /// Gets the index of the last article
        /// </summary>
        public int End { get { return start + itemCount < totalItems ? start + itemCount : totalItems; } }

        /// <summary>
        /// The number of total article.
        /// </summary>
        public int TotalItems { get { return totalItems; } }

// ReSharper disable once InconsistentNaming
        public ObservableCollection<ArticleTableInfo> _ResultTable;

        public ObservableCollection<ArticleTableInfo> _FillArticleTableInfo;

        #endregion
        RelayCommand _openFileCommand;
        RelayCommand _searchPubMedCommand;
        RelayCommand _openHelpDocumentCommand;
        RelayCommand _retrieveArticleInfoCommand;
        RelayCommand _exportCommand;
        private RelayCommand _ArticleInfotableClick;
        #endregion // Fields
         
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

                Process.Start(FillArticleTableInfo[SelectIndex].Url.ToString());
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
                _ResultTable.Add(new ArticleTableInfo() { ArticleTitle = myArticle[i], Author = myAuthor[i], Year = myYear[i], Journal = myJournal[i], Url = myUrl[i] });
            }
            totalItems = _ResultTable.Count;
        }

        #region Constructor

        public MainWindowViewModel()      
        {
            UserInputFromModel  = new UserInput();
            PubMedSearch = new PubMedDataRetrieval();
            CountListWithProteins = new ObservableCollection<HitCountTable>();
            CountList = new List<int>();
            this.RetrievalArticleInfo();
            Refresh();
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

        public ICommand RetrieveArticleInfo
        {
            get { return _openFileCommand ?? (_openFileCommand = new RelayCommand(OpenFile)); }
        }

        #endregion // Commands

        #region Methods
        public void GetCount()
        {
            // parse proteins string into a list
            ProteinList = Regex.Split(ProteinFromModel, "\n");

            foreach (string protein in ProteinList)
            {
                int count = PubMedSearch.GetCount(protein, OrganismFromModel, KeywordFromModel);
                CountList.Add(count);
                CountListWithProteins.Add(new HitCountTable(count, protein));
            }
            
        }

        public void OpenHelpDocument()
        {

            var helpDocumentWindow = new HelpDocumentView();
            helpDocumentWindow.Show();

        }
        #endregion
        
       

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
                            while (count < this._ResultTable.Count)
                            {
                                writer.WriteLine(this._ResultTable[count].ArticleTitle);
                                writer.WriteLine(this._ResultTable[count].Author);
                                writer.WriteLine(this._ResultTable[count].Journal);
                                writer.WriteLine(this._ResultTable[count].Year);
                                writer.WriteLine(this._ResultTable[count].Url);
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
                    if (firstCommand == null)
                    {
                        firstCommand = new RelayCommand_
                        (
                            param =>
                            {
                                start = 0;
                                Refresh();
                            },
                            param =>
                            {
                                return start - itemCount >= 0 ? true : false;
                            }
                        );
                    }

                    return firstCommand;
                }
            }

            /// <summary>
            /// Gets the command for moving to the previous page
            /// </summary>
            public ICommand PreviousCommand
            {
                get
                {
                    if (previousCommand == null)
                    {
                        previousCommand = new RelayCommand_
                        (
                            param =>
                            {
                                start -= itemCount;
                                Refresh();
                            },
                            param =>
                            {
                                return start - itemCount >= 0 ? true : false;
                            }
                        );
                    }

                    return previousCommand;
                }
            }

            /// <summary>
            /// Gets the command for moving to the next page 
            /// </summary>
            public ICommand NextCommand
            {
                get
                {
                    if (nextCommand == null)
                    {
                        nextCommand = new RelayCommand_
                        (
                            param =>
                            {
                                start += itemCount;
                                Refresh();
                            },
                            param =>
                            {
                                return start + itemCount < totalItems ? true : false;
                            }
                        );
                    }

                    return nextCommand;
                }
            }

            /// <summary>
            /// Gets the command for moving to the last page
            /// </summary>
            public ICommand LastCommand
            {
                get
                {
                    if (lastCommand == null)
                    {
                        lastCommand = new RelayCommand_
                        (
                            param =>
                            {
                                start = (totalItems / itemCount - 1) * itemCount;
                                start += totalItems % itemCount == 0 ? 0 : itemCount;
                                Refresh();
                            },
                            param =>
                            {
                                return start + itemCount < totalItems ? true : false;
                            }
                        );
                    }

                    return lastCommand;
                }
            }

            /// <summary>
            /// Refreshes the list article info
            /// </summary>
            private void Refresh()
            {
                FillPage(start);
                OnPropertyChanged("Start");
                OnPropertyChanged("End");
                OnPropertyChanged("TotalItems");
                OnPropertyChanged("FillArticleTableInfo");
            }

            public void FillPage(int startNum)
            {
                this._FillArticleTableInfo = new ObservableCollection<ArticleTableInfo>();
                for (int i = startNum; i < startNum + itemCount && i < totalItems; i++)

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
   
}
