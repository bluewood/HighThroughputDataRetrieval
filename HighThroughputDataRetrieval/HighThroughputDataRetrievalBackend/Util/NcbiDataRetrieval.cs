using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace HighThroughputDataRetrievalBackend.Util
{
    /// <summary>
    /// NcbiDataRetrieval is an abstract includes basic data members and methods 
    /// for web-biological repositories class  such as PubMedDataRetrieval. 
    /// NcbiDataRetrieval declares basic data members and
    /// methods for the child classes and the child classes will 
    /// </summary>
    public abstract class NcbiDataRetrieval
    {
        #region Properities

        public int QueryID { set; get; }
        public int QueryArticleID { set; get; }
        public int KeywordID { set; get; }
        public int OrganismID { set; get; }
        public int QuerySessionID { set; get; }
        public int ProteinID { set; get; }
        public int ProteinListID { set; get; }


        public List<string> IdList { set; get; }
        public int ArticleCount { set; get; }
        public string ProteinFromUser { set; get; }
        public string OrganismFromUser { set; get; }
        public string KeywordFromUser { set; get; }

        // datatables 
        public DataTable QueryDataTable { set; get; }
        public DataTable QueryArticlesDataTable { set; get; }
        public DataTable KeywordDataTable { set; get; }
        public DataTable OrganismDataTable { set; get; }
        public DataTable ProteinDataTable { set; get; }
        public DataTable ProteinListDataTable { set; get; }
        public DataTable QuerySessionDataTable { set; get; }
        public DataTable ArticleDataTable { set; get; }
        public DataTable AuthorListDataTable { set; get; }
        public DataTable AuthorsDataTable { set; get; }
        public DataTable JournalReleaseDataTable { set; get; }
        public DataTable JournalDataTable { set; get; }
       
        // Dataset to include the datatables
        public DataSet QueryArticlesDataSet { set; get; }

        // Dictionary to avoid duplication
        public static Dictionary<string, DataRow> Dictionary { set; get; }

        #endregion // properties


        #region Methods

        public abstract int GetCount(string protein, string organism, string keyword);
        public abstract bool FillQueryDataTables(string name, XmlNodeList pmidList);
        public abstract DataTable GetArticleInfomation(int count, List<string> idList);
        public abstract bool FillArticleDataTables(XmlNodeList articleList);
        public abstract DataSet GetDataSet();

        #endregion // methods


    }
}
