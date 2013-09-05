using System.Collections.Generic;
using System.Text.RegularExpressions;
using HighThroughputDataRetrievalBackend.Util;

namespace HighThroughputDataRetrieval.ViewModel
{
    public class HitCountTableViewModel
    {
        #region Fields

        public List<int> CountNumbers { get; set; }
        private readonly IEnumerable<string> _proteinList;
        private readonly string _organism;
        private readonly string _keyword;

        #endregion // Fields

        #region Constructor
        public HitCountTableViewModel(string proteinListString, string organism, string keyword)
        {

            _proteinList = ParseInputProteinString(proteinListString);
            _organism    = organism;
            _keyword     = keyword;
        }

        #endregion // Constructor

        #region Methods

        private static IEnumerable<string> ParseInputProteinString(string proteinListString)
        {
            IEnumerable<string> proteinList = Regex.Split(proteinListString, "\n");
            return proteinList;
        }

        public void GetCountNumber()
        {
            CountNumbers = new List<int>();
            NcbiDataRetrieval pubMedSearch = new PubMedDataRetrieval();

            // Get a list of count numbers of each protein + organism + keyword
            foreach (string protein in _proteinList)
            {
                // TODO: Fix this
                //CountNumbers.Add(pubMedSearch.getPubMedCount(protein, _organism, _keyword));
            }

           // DataSet dsArticles = pubMedSearch.getArticleInfo();
           // PubMedSearch.s_IdList
           //data_base.create_data("C:/Users/Nan/Desktop/new_file.db3");
           // data_base.CopydatasetToDatabase("C:/Users/Nan/Desktop/new_file.db3", ds_Articles);           
        }        

        #endregion
    }
}