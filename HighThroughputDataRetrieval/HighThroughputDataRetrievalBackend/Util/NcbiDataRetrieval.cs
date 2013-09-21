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
        protected List<string> IdList;
        //protected XmlNodeList ArticleList;
        protected int RetrievedArticleCount;
        protected int KeyOrder;
        protected string ProteinFromUser;
        protected string OrganismFromUser;
        protected string KeywordFromUser;

        // datatables 
        protected DataTable QueryDataTable;
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
        protected DataSet QueryArticlesDataSet;

        // Dictionary to avoid duplication
        protected static Dictionary<string, DataRow> Dictionary;

        #endregion


        #region Methods
        public abstract int GetCountAndIds(string protein, string organism, string keyword);
        public abstract void FillQueryDatatables(string name, int count, XmlNodeList pmidList);
        public abstract DataTable GetArticleInfomation();
        public abstract DataSet GetDataSet();
        public abstract void FillAticleInfoDatatables(XmlNodeList articleList, string pubmedSearchPrefix);
        
        //public abstract bool inputTag();
        //public abstract bool categorizeArticle();

        #endregion


    }
}
