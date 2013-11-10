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

        // List for PMIDs and xmlList for articles
        public List<string> IdList { set; get; }
        //protected XmlNodeList ArticleList;
        //public int RetrievedArticleCount { set; get; }
        public int KeyOrder { set; get; }
        public string ProteinFromUser { set; get; }
        public string OrganismFromUser { set; get; }
        public string KeywordFromUser { set; get; }

        // datatables 
        public DataTable QueryDataTable { set; get; }
        public DataTable QueryArticlesDataTable { set; get; }
        public DataTable KeywordListDataTable { set; get; }
        public DataTable KeywordDataTable { set; get; }
        public DataTable OrganismDataTable { set; get; }
        public DataTable ProteinsDataTable { set; get; }
        public DataTable ProteinListDataTable { set; get; }
        public DataTable QuerySessionDataTable { set; get; }
        public DataTable AlternativeProteinNameDataTable { set; get; }
        public DataTable ArticleDataTable { set; get; }
        public DataTable AuthorListDataTable { set; get; }
        public DataTable AuthorsDataTable { set; get; }
        public DataTable JournalReleaseDataTable { set; get; }
        public DataTable JournalDataTable { set; get; }
        public DataTable ArticleInfoDataTable { set; get; }
        //public DataTable TagListDataTable;
        //public DataTable TagsDataTable;
        //public DataTable CategoryListDataTable;
        //public DataTable CategoriesDataTable;

        // Dataset to include the datatables
        public DataSet QueryArticlesDataSet { set; get; }

        // Dictionary to avoid duplication
        public static Dictionary<string, DataRow> Dictionary { set; get; }

        #endregion // properties


        #region Methods

        public abstract int GetCount(string protein, string organism, string keyword);
        public abstract void FillQueryDataTables(string name, int count, XmlNodeList pmidList);
        public abstract DataTable GetArticleInfomation(int count, List<string> idList);
        public abstract void FillArticleDataTables(XmlNodeList articleList, string pubmedSearchPrefix);
        public abstract DataSet GetDataSet();


        #endregion // methods


    }
}
