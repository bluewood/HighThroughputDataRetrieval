using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Xml;

namespace HighThroughputDataRetrievalBackend.Util
{
    /// <summary>
    /// PubMedDataRetrieval is inherited from the parent class, NcbiDataRetrieval, which declares
    /// all basic data members and methods
    /// </summary>
    public class PubMedDataRetrieval : NcbiDataRetrieval
    {
        // constructor: initialize all properties 
        public PubMedDataRetrieval()
        {
            IdList = new List<string>();
            //RetrievedArticleCount = 0;
            KeyOrder = 1;
            ProteinFromUser = "";
            OrganismFromUser = "";
            KeywordFromUser = "";          


            /* initialize datatables and columns */            
            QueryDataTable = new DataTable("T_Query");
            QueryDataTable.Columns.Add("QueryID", typeof(int));
            QueryDataTable.Columns.Add("Name");
            //dt_Query.Columns.Add("Description"); what information for???
            //QueryDataTable.Columns.Add("QueryStartTime");
            //QueryDataTable.Columns.Add("QueryEndTime");
            QueryDataTable.Columns.Add("ProteinID", typeof(int));
            QueryDataTable.Columns.Add("OrganismID", typeof(int));
            QueryDataTable.Columns.Add("KeywordListID", typeof(int));
            QueryDataTable.Columns.Add("ResultCount", typeof(int));
            QueryDataTable.PrimaryKey = new[] {QueryDataTable.Columns["QueryID"] };


            QueryArticlesDataTable = new DataTable("T_QueryArticles");
            QueryArticlesDataTable.Columns.Add("QueryArticleID", typeof(int));
            QueryArticlesDataTable.Columns.Add("QueryID", typeof(int));
            QueryArticlesDataTable.Columns.Add("PMID"); // should be int or string???
            QueryArticlesDataTable.PrimaryKey = new[] { QueryArticlesDataTable.Columns["QueryArticleID"] };


            KeywordListDataTable = new DataTable("T_KeyWordList");
            KeywordListDataTable.Columns.Add("KeywordListID", typeof(int));
            KeywordListDataTable.Columns.Add("KeywordOrder", typeof(int));
            KeywordListDataTable.Columns.Add("KeywordID", typeof(int));
            KeywordListDataTable.PrimaryKey = new [] { KeywordListDataTable.Columns["KeywordListID"]};


            KeywordDataTable = new DataTable("T_Keyword");
            KeywordDataTable.Columns.Add("KeywordID", typeof(int));
            KeywordDataTable.Columns.Add("Keyword");
            KeywordDataTable.PrimaryKey = new [] { KeywordDataTable.Columns["KeywordID"] };


            OrganismDataTable = new DataTable("T_Organism");
            OrganismDataTable.Columns.Add("OrganismID", typeof(int));
            OrganismDataTable.Columns.Add("Organism");
            OrganismDataTable.PrimaryKey = new [] { OrganismDataTable.Columns["OrganismID"] };
             

            QuerySessionDataTable = new DataTable("T_QuerySession");
            QuerySessionDataTable.Columns.Add("QuerySessionID", typeof(int));
            QuerySessionDataTable.Columns.Add("QueryID", typeof(int));
            QuerySessionDataTable.Columns.Add("ProteinID", typeof(int));
            QuerySessionDataTable.Columns.Add("DateTime", typeof(DateTime));
            QuerySessionDataTable.PrimaryKey = new [] { QuerySessionDataTable.Columns["QuerySessionID"] };


            ProteinsDataTable = new DataTable("T_Protein");
            ProteinsDataTable.Columns.Add("ProteinID", typeof(int));
            ProteinsDataTable.Columns.Add("Protein");
            ProteinsDataTable.PrimaryKey = new [] { ProteinsDataTable.Columns["ProteinID"]};

            ProteinListDataTable = new DataTable("ProteinList");
            ProteinListDataTable.Columns.Add("ProteinListID", typeof(int));
            ProteinListDataTable.Columns.Add("QuerySessionID", typeof(int));
            ProteinListDataTable.Columns.Add("ProteinID", typeof(int));
            ProteinListDataTable.PrimaryKey = new [] { ProteinListDataTable.Columns["ProteinListID"]};


            //AlternativeProteinNameDataTable = new DataTable("T_AlternativeProteinName");
            //AlternativeProteinNameDataTable.Columns.Add("AlternativeProteinID");
            //AlternativeProteinNameDataTable.Columns.Add("AlternativeProteinName");
            //AlternativeProteinNameDataTable.Columns.Add("ProteinID");
            //AlternativeProteinNameDataTable.PrimaryKey = new [] { AlternativeProteinNameDataTable.Columns["AlternativeProteinID"] };
                  
           
            ArticleDataTable = new DataTable("T_Article");
            ArticleDataTable.Columns.Add("PMID");
            ArticleDataTable.Columns.Add("AuthorListID", typeof(int));
            ArticleDataTable.Columns.Add("Title");
            ArticleDataTable.Columns.Add("PubDate");
            ArticleDataTable.Columns.Add("Language");
            ArticleDataTable.Columns.Add("doi");
            ArticleDataTable.Columns.Add("Abstract");
            ArticleDataTable.Columns.Add("JournalRelease");
            ArticleDataTable.Columns.Add("Pages");
            ArticleDataTable.Columns.Add("Affiliation");
            ArticleDataTable.Columns.Add("URL");
            ArticleDataTable.PrimaryKey = new [] { ArticleDataTable.Columns["PMID"], 
                                                    ArticleDataTable.Columns["AuthorListID"] };

        
            AuthorListDataTable = new DataTable("T_AuthorList");
            AuthorListDataTable.Columns.Add("PMID");
            AuthorListDataTable.Columns.Add("AuthorListID", typeof(int));
            AuthorListDataTable.Columns.Add("AuthorID",typeof(int));
            AuthorListDataTable.Columns.Add("AuthorOrder", typeof(int));
            //AuthorListDataTable.PrimaryKey = new[] { AuthorListDataTable.Columns["PMID"], 
            //                                            AuthorListDataTable.Columns["AuthorListID"], 
            //                                            AuthorListDataTable.Columns["AuthorID"] };
           
            AuthorsDataTable = new DataTable("T_Authors");
            AuthorsDataTable.Columns.Add("AuthorID", typeof(int));
            AuthorsDataTable.Columns.Add("Suffix");
            AuthorsDataTable.Columns.Add("LastName");
            AuthorsDataTable.Columns.Add("ForeName");
            AuthorsDataTable.Columns.Add("Initials");
            AuthorsDataTable.Columns.Add("CollectiveName"); 
            AuthorsDataTable.PrimaryKey = new[] { AuthorsDataTable.Columns["AuthorID"] };

            JournalReleaseDataTable = new DataTable("T_JournalRelease");
            JournalReleaseDataTable.Columns.Add("JournalRelease");
            JournalReleaseDataTable.Columns.Add("JournalID", typeof(int));
            JournalReleaseDataTable.Columns.Add("Year"); // should be int or string???
            JournalReleaseDataTable.Columns.Add("Volume");
            JournalReleaseDataTable.Columns.Add("Issue");
            JournalReleaseDataTable.PrimaryKey = new[] { JournalReleaseDataTable.Columns["JournalRelease"], 
                                                            JournalReleaseDataTable.Columns["JournalID"] };
           
            JournalDataTable = new DataTable("T_Journals");
            JournalDataTable.Columns.Add("JournalID", typeof(int));
            JournalDataTable.Columns.Add("Title");
            JournalDataTable.PrimaryKey = new[] { JournalDataTable.Columns["JournalID"] };
         

            //ArticleInfoDataTable = new DataTable("T_ArticleInfo");
            //ArticleInfoDataTable.Columns.Add("PMID");
            //ArticleInfoDataTable.Columns.Add("TagListID");
            //ArticleInfoDataTable.Columns.Add("CategoryListID");
            //ArticleInfoDataTable.Columns.Add("NotesID");
            //ArticleInfoDataTable.Columns.Add("URL");
            //ArticleInfoDataTable.PrimaryKey = new[] { ArticleInfoDataTable.Columns["PMID"], 
            //                                            ArticleInfoDataTable.Columns["TagListID"], 
            //                                            ArticleInfoDataTable.Columns["CategoryListID"] };

            //TagListDataTable = new DataTable("T_TagList");
            //TagListDataTable.Columns.Add("TagListID");
            //TagListDataTable.Columns.Add("TagID");
            //TagListDataTable.PrimaryKey = new[] { TagListDataTable.Columns["TagListID"]};


            //TagsDataTable = new DataTable("T_Tags");
            //TagsDataTable.Columns.Add("TagID");
            //TagsDataTable.Columns.Add("Tag");
            //TagsDataTable.PrimaryKey = new[] { TagsDataTable.Columns["TagID"] };


            //CategoryListDataTable = new DataTable("T_CategoryList");
            //CategoryListDataTable.Columns.Add("CategoryListID");
            //CategoryListDataTable.Columns.Add("CategoryID");
            //CategoryListDataTable.PrimaryKey = new[] { CategoryListDataTable.Columns["CategoryListID"]};


            //CategoriesDataTable = new DataTable("T_Categories");
            //CategoriesDataTable.Columns.Add("CategoryID");
            //CategoriesDataTable.Columns.Add("Category");
            //CategoriesDataTable.PrimaryKey = new[] { CategoriesDataTable.Columns["CategoryID"]};


            QueryArticlesDataSet = new DataSet("QueryArticlesSet");


            Dictionary = new Dictionary<string,DataRow>();

        }


        // Name: getCout()
        // Parameters: one protein, one organism, and keywords list of string type 
        // Funtion: Retrieve number of articles and PMIDs and put them into the dataset
        //          based on one protein, one organizm, and one keyword at a time. 9/6/2013
        // Return: count number of int type
        override public int GetCount(string protein, string organism, string  keyword)
        {
            ProteinFromUser = protein;
            OrganismFromUser = organism;
            KeywordFromUser = keyword;

            #region Members
            
            const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils//esearch.fcgi?db=pubmed&retmax=10000&";

            // combine proteins and keyword
            string terms = string.Format("{0},{1},{2}", ProteinFromUser, OrganismFromUser, KeywordFromUser);

            string assembleUrl = string.Format("{0}&term={1}",pubmedSearchPrefix,terms);

            #endregion


            // connect pubmed and retrieve the information that are count number and PMID list.
            var client = new WebClient();
            string urlResult = "";
            try
            {

                urlResult = client.DownloadString(assembleUrl);

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            //Create the XmlDocument and load the resut as xml format strings.
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(urlResult);

            // name is the name in T_Query table and needed to check duplication query
            string name = protein + organism + keyword;

            // get count from xml string
            int count = int.Parse(xmlDocument.GetElementsByTagName("Count")[0].InnerText);

            //T_Query, if query already exists and the count number in the data table is also same 
            //return the count number 
            DataRow rowQuery;
            if (Dictionary.TryGetValue(name, out rowQuery) && rowQuery["ResultCount"].Equals(count)) // count == rowQuery.Field<int>("ResultCount") ??? need to test
            {
                // already exists
                // return int.Parse(rowQuery["ResultCount"].ToString());
                return rowQuery.Field<int>("ResultCount");
            }

            // get PMIDs from xml string in the XmlNodeList
            XmlNodeList pmidListFromXml = xmlDocument.GetElementsByTagName("Id");

            // fill Query, Protein, Organism, Keyword, KeywordList, QueryArticle, QuerySession datatables
            FillQueryDataTables(name, count, pmidListFromXml);

            return count;  

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="pmidListFromXml"></param>
        override public void FillQueryDataTables(string name, int count, XmlNodeList pmidListFromXml)
        {
            try
            {
                // T_Protein 
                DataRow rowProtein;
                if (!(Dictionary.TryGetValue(ProteinFromUser, out rowProtein)))
                {
                    // not exist -> create new row of T_Protein and fill it.
                    rowProtein = ProteinsDataTable.NewRow();
                    rowProtein["ProteinID"] = ProteinsDataTable.Rows.Count + 1;
                    rowProtein["Protein"] = ProteinFromUser;

                    // add the row into the T_Protein data table and the dictionary
                    ProteinsDataTable.Rows.Add(rowProtein);
                    Dictionary.Add(ProteinFromUser, rowProtein);
                }

                // T_Organism
                DataRow rowOrganism;
                if (!(Dictionary.TryGetValue(OrganismFromUser, out rowOrganism)))
                {
                    // not exist -> create new row of T_Organism and fill it.
                    rowOrganism = OrganismDataTable.NewRow();
                    rowOrganism["OrganismID"] = OrganismDataTable.Rows.Count + 1;
                    rowOrganism["Organism"] = OrganismFromUser;

                    // add the row into T_Organism data table and the dictionary
                    OrganismDataTable.Rows.Add(rowOrganism);
                    Dictionary.Add(OrganismFromUser, rowOrganism);
                }


                // T_Keyword
                DataRow rowKeyword;
                if (!(Dictionary.TryGetValue(KeywordFromUser, out rowKeyword)))
                {
                    // not exist -> create new row of T_Keyword and fill it
                    rowKeyword = KeywordDataTable.NewRow();
                    rowKeyword["KeywordID"] = KeywordDataTable.Rows.Count + 1;
                    rowKeyword["Keyword"] = KeywordFromUser;

                    // add the row into T_Keyword data table and the dictionary.
                    KeywordDataTable.Rows.Add(rowKeyword);
                    Dictionary.Add(KeywordFromUser, rowKeyword);

                }


                // T_KeywordList          
                //DataRow rowKeywordList = KeywordListDataTable.NewRow();
                //rowKeywordList["KeywordListID"] = QueryDataTable.Rows.Count + 1;
                //rowKeywordList["KeywordOrder"] = KeyOrder++;
                //rowKeywordList["KeywordID"] = rowKeyword["KeywordID"]; //.ToString();
                //KeywordListDataTable.Rows.Add(rowKeywordList);


                // The rest of T_Query
                DataRow rowQuery = QueryDataTable.NewRow();
                rowQuery["QueryID"] = QueryDataTable.Rows.Count + 1;
                rowQuery["Name"] = name; // name = protein + organism + keyword
                //rowQuery["Description"] = ; ???
                //rowQuery["QueryStartTime"]=;
                //rowQuery["QueryEndTime"]=;
                rowQuery["ProteinID"] = rowProtein["ProteinID"]; //.ToString();
                rowQuery["OrganismID"] = rowOrganism["OrganismID"]; //.ToString();
                rowQuery["KeywordListID"] = rowQuery["QueryID"]; //.ToString(); // keywordListID = QueryID because one query has one keywordList
                rowQuery["ResultCount"] = count; // new count from the xml string

                // Add the row of T_Query into the T_Query datatable and the dictionary
                QueryDataTable.Rows.Add(rowQuery);
                Dictionary.Add(name, rowQuery);


                // T_QueryArticles            
                foreach (XmlNode pmid in pmidListFromXml)
                {
                    // create the row of QueryArticles and fill it.
                    DataRow rowQueryArticles = QueryArticlesDataTable.NewRow();
                    rowQueryArticles["QueryArticleID"] = QueryArticlesDataTable.Rows.Count + 1;
                    rowQueryArticles["QueryID"] = rowQuery["QueryID"];
                    rowQueryArticles["PMID"] = pmid.InnerText;

                    // add pmid into ldlist and the row into datatable
                    IdList.Add(pmid.InnerText);
                    QueryArticlesDataTable.Rows.Add(rowQueryArticles);

                }

                // T_QuerySession
                // create new row, fill information, and add the row into the QuerySession data table
                DataRow rowQuerySession = QuerySessionDataTable.NewRow();
                rowQuerySession["QuerySessionID"] = QuerySessionDataTable.Rows.Count + 1;
                rowQuerySession["QueryID"] = rowQuery["QueryID"];
                rowQuerySession["ProteinID"] = rowQuery["ProteinID"];
                rowQuerySession["DateTime"] = DateTime.Now;
                QuerySessionDataTable.Rows.Add(rowQuerySession);
            }
            catch (Exception exception)
            {
                
                Console.WriteLine(exception.Message);
            }
 
            //T_ProteinList ???         
            //T_AlternativeProteinName ???
        }
        
        // Name: getArticleInfo()
        // Parameters: N/a
        // Function: Retrieve specific articles' information based on the PMIDs from the web-repositories
        //         and put those information into the dataset.
        // Return: null if it fails and a filled dataset if it successes.
        /// <summary>
        ///  ??? Need to consider retrieving every 20 articles 
        ///  
        /// </summary>
        /// <returns></returns>
        override public DataTable GetArticleInfomation(int count, List<string> idList)
        {               

            int i = 0;
            while(i<count)
            {
                #region Members
                const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=";
                #endregion

                // the Url is too long when retrieving articles over 100 at a time, so occur exception
                // so retrieving 20 articles at a time until retrieving number of count
                int numberOfRetrieving = ((count-i) < 20)? (count-i):20;
                
                // get 20 ids into string
                string ids = string.Join(",", idList.GetRange(i, numberOfRetrieving));
                i += numberOfRetrieving;

                // make url
                string assembleUrl = string.Format("{0}{1}&{2}", pubmedSearchPrefix, ids, "retmode=xml");
                //Console.WriteLine(assembleUrl);

                // Retrieve article information from PubMed through the URL
                var client = new WebClient();
                string urlResult = "";
                try
                {
                    urlResult = client.DownloadString(assembleUrl);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }

                // load result in xmlformat and parse per article
                var doc = new XmlDocument();
                doc.LoadXml(urlResult);
                XmlNodeList articleListFromXml = doc.GetElementsByTagName("PubmedArticle");

                //// fill article part datatables
                FillArticleDataTables(articleListFromXml, pubmedSearchPrefix);
                
            }
            
   

           return ArticleDataTable;

        }

        public override void FillArticleDataTables(XmlNodeList articleListFromXml, string pubmedSearchPrefix)
        {
            try
            {
                
                // fill the data tables from XMLNodeList
                foreach (XmlNode article in articleListFromXml)
                {

                    /* T_Article */
                    string pmid = article.SelectSingleNode("descendant::PMID").InnerText;

                    // check article already exists. if yes, move to the next article
                    if (Dictionary.ContainsKey(pmid))
                    {
                        Console.WriteLine(pmid);
                        continue;
                    }
                        

                    // some information doesn't show in the xml, so need to check if it's null or not.
                    DataRow rowArticle = ArticleDataTable.NewRow();
                    rowArticle["PMID"] = pmid;
                    rowArticle["AuthorListID"] = ArticleDataTable.Rows.Count + 1;
                    rowArticle["Title"] = article.SelectSingleNode("descendant::ArticleTitle").InnerText;
                    rowArticle["PubDate"] = article.SelectSingleNode("descendant::ArticleDate") != null
                        ? article.SelectSingleNode("descendant::ArticleDate").InnerText
                        : null;
                    rowArticle["Language"] = article.SelectSingleNode("descendant::Language") != null
                        ? article.SelectSingleNode("descendant::Language").InnerText
                        : null;
                    rowArticle["doi"] = article.SelectSingleNode("descendant::ArticleIdList/ArticleId[@IdType='doi']") !=
                                        null
                        ? article.SelectSingleNode("descendant::ArticleIdList/ArticleId[@IdType='doi']").InnerText
                        : null;
                    rowArticle["Abstract"] = article.SelectSingleNode("descendant::Abstract") != null
                        ? article.SelectSingleNode("descendant::Abstract").InnerText
                        : null;
                    // ??? What is journal release???
                    rowArticle["JournalRelease"] = article.SelectSingleNode("descendant::ISSN") != null
                        ? article.SelectSingleNode("descendant::ISSN").InnerText
                        : null;
                    rowArticle["Pages"] = article.SelectSingleNode("descendant::MedlinePgn") != null
                        ? article.SelectSingleNode("descendant::MedlinePgn").InnerText
                        : null;
                    rowArticle["Affiliation"] = article.SelectSingleNode("descendant::Affiliation") != null
                        ? article.SelectSingleNode("descendant::Affiliation").InnerText
                        : null;
                    string url = string.Format("{0}{1}&{2}", pubmedSearchPrefix, rowArticle["PMID"], "retmode=xml");
                    rowArticle["URL"] = url;

                    // add a row into the T_Article and the dictionary
                    ArticleDataTable.Rows.Add(rowArticle);
                    Dictionary.Add(pmid, rowArticle);


                    /* T_Authors and T_AuthorList*/
                    int authorOrder = 1;
                    XmlNodeList authorList = article.SelectNodes("descendant::Author");

                    foreach (XmlNode author in authorList)
                    {
                        string lastName = author.SelectSingleNode("descendant::LastName") != null
                            ? author.SelectSingleNode("descendant::LastName").InnerText
                            : null;
                        string foreName = author.SelectSingleNode("descendant::ForeName") != null
                            ? author.SelectSingleNode("descendant::ForeName").InnerText
                            : null;
                        string authorName = lastName + foreName;

                        // We currently do not count different persons with the same name.
                        // what if user restarts the application? Do I have to start authorID as 1 again?
                        // I may need to get author information (id and name) first from the database.
                        // chech the author already exists. If yes, move to the next the author
                        if (Dictionary.ContainsKey(authorName))
                            continue;


                        DataRow rowAuthor = AuthorsDataTable.NewRow();
                        rowAuthor["AuthorID"] = AuthorsDataTable.Rows.Count + 1;
                        rowAuthor["Suffix"] = author.SelectSingleNode("descendant::Suffix") != null
                            ? author.SelectSingleNode("descendant::Suffix").InnerText
                            : null;
                        rowAuthor["LastName"] = lastName;
                        rowAuthor["ForeName"] = foreName;
                        rowAuthor["Initials"] = author.SelectSingleNode("descendant::Initials") != null
                            ? author.SelectSingleNode("descendant::Initials").InnerText
                            : null;
                        rowAuthor["CollectiveName"] = author.SelectSingleNode("descendant::CollectiveName") != null
                            ? author.SelectSingleNode("descendant::CollectiveName").InnerText
                            : null;

                        // add the row into the T_Author and the dictionary
                        AuthorsDataTable.Rows.Add(rowAuthor);
                        Dictionary.Add(authorName, rowAuthor);


                        // add a row into the T_AuthorList
                        DataRow rowAuthorList = AuthorListDataTable.NewRow();
                        rowAuthorList["PMID"] = pmid;
                        rowAuthorList["AuthorListID"] = ArticleDataTable.Rows.Count;
                        rowAuthorList["AuthorID"] = AuthorsDataTable.Rows.Count;
                        rowAuthorList["AuthorOrder"] = authorOrder++;
                        AuthorListDataTable.Rows.Add(rowAuthorList);


                    } // end of inner foreach T_Authors and T_AuthorList



                    // T_Journals
                    string journalTitle = article.SelectSingleNode("descendant::Title") != null
                        ? article.SelectSingleNode("descendant::Title").InnerText
                        : null;
                    DataRow rowJournals;

                    // not exists, return false --> fill the datatable
                    // exists, return true with rowJournal
                    if (!(Dictionary.TryGetValue(journalTitle, out rowJournals)))
                    {
                        rowJournals = JournalDataTable.NewRow();
                        rowJournals["JournalID"] = JournalDataTable.Rows.Count + 1;
                        rowJournals["Title"] = journalTitle;

                        JournalDataTable.Rows.Add(rowJournals);
                        Dictionary.Add(journalTitle, rowJournals);
                    }

                    // T_JournalRelease
                    string journalRelease = rowArticle["JournalRelease"].ToString();
                    if (Dictionary.ContainsKey(journalRelease))
                        continue;

                    DataRow rowJournalRelease = JournalReleaseDataTable.NewRow();
                    rowJournalRelease["JournalRelease"] = journalRelease;
                    rowJournalRelease["JournalID"] = rowJournals["JournalID"].ToString();
                    rowJournalRelease["Year"] = article.SelectSingleNode("descendant::PubDate") != null
                        ? article.SelectSingleNode("descendant::PubDate").InnerText
                        : null;
                    rowJournalRelease["Volume"] = article.SelectSingleNode("descendant::Volume") != null
                        ? article.SelectSingleNode("descendant::Volume").InnerText
                        : null;
                    rowJournalRelease["Issue"] = article.SelectSingleNode("descendant::Issue") != null
                        ? article.SelectSingleNode("descendant::Issue").InnerText
                        : null;


                    JournalReleaseDataTable.Rows.Add(rowJournalRelease);
                    Dictionary.Add(rowJournalRelease["JournalRelease"].ToString(), rowJournalRelease);

                } // end of outer foreach
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
          
        }

        public override DataSet GetDataSet()
        {
            // add the datatables in the dataset
            QueryArticlesDataSet.Tables.Add(QueryDataTable);
            QueryArticlesDataSet.Tables.Add(ProteinsDataTable);
            QueryArticlesDataSet.Tables.Add(OrganismDataTable);
            QueryArticlesDataSet.Tables.Add(KeywordDataTable);
            //QueryArticlesDataSet.Tables.Add(KeywordListDataTable);
            QueryArticlesDataSet.Tables.Add(QueryArticlesDataTable);
            QueryArticlesDataSet.Tables.Add(QuerySessionDataTable);
            //ds_Article.Tables.Add(dt_ProteinList);
            //ds_Article.Tables.Add(dt_AlternativeProteinName);                                               
            QueryArticlesDataSet.Tables.Add(ArticleDataTable);
            QueryArticlesDataSet.Tables.Add(AuthorListDataTable);
            QueryArticlesDataSet.Tables.Add(AuthorsDataTable);
            QueryArticlesDataSet.Tables.Add(JournalDataTable);
            QueryArticlesDataSet.Tables.Add(JournalReleaseDataTable);

            // check any datatable in the dataset is empty
            if (QueryArticlesDataSet.Tables.Cast<DataTable>().Any(datatable => datatable.Rows.Count == 0))
            {
                return null;
            }

            //return QueryArticlesDataSet;
            return QueryArticlesDataSet;
        }
    }
}
