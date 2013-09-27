using System.Collections.Generic;
using System.Data;
using System.Security.RightsManagement;
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
        protected List<string> IdList { set; get; }
        //protected XmlNodeList ArticleList;
        protected int RetrievedArticleCount { set; get; }
        protected int KeyOrder { set; get; }
        protected string ProteinFromUser { set; get; }
        protected string OrganismFromUser { set; get; }
        protected string KeywordFromUser { set; get; }

        // datatables 
        public DataTable QueryDataTable;
        protected DataTable QueryArticlesDataTable;
        protected DataTable KeywordListDataTable;
        protected DataTable KeywordDataTable;
        protected DataTable OrganismDataTable;
        protected DataTable ProteinsDataTable;
        protected DataTable ProteinListDataTable;
        protected DataTable QuerySessionDataTable;
        protected DataTable AlternativeProteinNameDataTable;
        protected DataTable ArticleDataTable;
        protected DataTable AuthorListDataTable;
        protected DataTable AuthorsDataTable;
        protected DataTable JournalReleaseDataTable;
        protected DataTable JournalDataTable;
        protected DataTable ArticleInfoDataTable;
        //public DataTable TagListDataTable;
        //public DataTable TagsDataTable;
        //public DataTable CategoryListDataTable;
        //public DataTable CategoriesDataTable;

        // Dataset to include the datatables
        public DataSet QueryArticlesDataSet;

        // Dictionary to avoid duplication
        protected static Dictionary<string, DataRow> Dictionary;

        #endregion


        #region Methods
        public abstract int GetCount(string protein, string organism, string keyword);
        public abstract List<string> GetIdList();
        public abstract void FillQueryDataTables(string name, int count, XmlNodeList pmidList);
        public abstract DataTable GetArticleInfomation();
        public abstract DataSet GetDataSet();
        public abstract void FillArticleDataTables(XmlNodeList articleList, string pubmedSearchPrefix);



        #endregion


    }
}
