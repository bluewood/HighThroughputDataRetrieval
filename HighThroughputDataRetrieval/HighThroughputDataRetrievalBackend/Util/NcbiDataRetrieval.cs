using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace HighThroughputDataRetrievalBackend.Util
{
    public abstract class NcbiDataRetrieval
    {
        #region Properities

        // List for PMIDs and xmlList for articles
        public List<string> IdList;
        public XmlNodeList ArticleList;
        public int KeyOrder;

        // datatables 
        public DataTable QueryDataTable;
        public DataTable QueryArticlesDataTable;
        public DataTable KeywordListDataTable;
        public DataTable KeywordDataTable;
        public DataTable OrganismDataTable;
        public DataTable ProteinsDataTable;
        public DataTable ProteinListDataTable;
        public DataTable QuerySessionDataTable;
        public DataTable AlternativeProteinNameDataTable;
        public DataTable ArticleDataTable;
        public DataTable AuthorListDataTable;
        public DataTable AuthorsDataTable;
        public DataTable JournalReleaseDataTable;
        public DataTable JournalDataTable;
        public DataTable ArticleInfoDataTable;
        //public DataTable TagListDataTable;
        //public DataTable TagsDataTable;
        //public DataTable CategoryListDataTable;
        //public DataTable CategoriesDataTable;

        // Dataset to include the datatables
        public DataSet QueryArticlesDataSet;

        // Dictionary to avoid duplication
        public Dictionary<string, DataRow> Dictionary;

        #endregion


        #region Methods
        public abstract int GetCount(string protein, string organism, string keyword);
        //public abstract bool ParseXml();
        public abstract DataSet GetArticleInfo();
        //public abstract bool inputTag();
        //public abstract bool categorizeArticle();
        #endregion


    }
}
