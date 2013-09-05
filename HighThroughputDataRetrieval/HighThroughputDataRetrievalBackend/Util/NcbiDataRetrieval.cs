using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace HighThroughputDataRetrievalBackend.Util
{
    public abstract class NcbiDataRetrieval
    {
        // properties
        public List<string> IdList;
        public XmlNodeList ArticleList;


        // 19 datatables 
        public DataTable dt_Query;
        public DataTable dt_QueryArticles;
        public DataTable dt_KeywordList;
        public DataTable dt_Keyword;
        public DataTable dt_Organism;
        public DataTable dt_QuerySession;
        public DataTable dt_Protein;
        public DataTable dt_ProteinList;
        public DataTable dt_AlternativeProteinName;
        public DataTable dt_Article;
        public DataTable dt_AuthorList;
        public DataTable dt_Authors;
        public DataTable dt_Journal_Release;
        public DataTable dt_Journal;
        public DataTable dt_ArticleInfo;
        public DataTable dt_TagList;
        public DataTable dt_Tags;
        public DataTable dt_CategoryList;
        public DataTable dt_Categories;

        // Dataset to include the 19 datatables
        public DataSet ds_Article;

        // Dictionary to avoid duplication
        public Dictionary<string, DataRow> Dict;


        // methods
        public abstract int GetCount(string pro, string org, List<string> keys);
        //public abstract bool storeInDataset(string s_PrefixURL);
        public abstract DataSet GetArticleInfo();
        //public abstract bool inputTag();
        //public abstract bool categorizeArticle();


    }
}
