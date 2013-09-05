using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace HighThroughputDataRetrievalBackend.Util
{
    public class PubMedDataRetrieval : NcbiDataRetrieval
    {
        // constructor: initialize all properties and create columns
        public PubMedDataRetrieval()
        {
            //i_KeyOrder = 0;

            /* initialize datatables and columns */            
            dt_Query = new DataTable("T_Query");
            dt_Query.Columns.Add("QueryID");
            dt_Query.Columns.Add("Name");
            //dt_Query.Columns.Add("Description"); what information for???
            dt_Query.Columns.Add("ProteinID");
            dt_Query.Columns.Add("OrganismID");
            dt_Query.Columns.Add("KeywordListID");
            dt_Query.Columns.Add("ResultCount");
            dt_Query.PrimaryKey = new DataColumn[] { dt_Query.Columns["QueryID"]};


            dt_QueryArticles = new DataTable("T_QueryArticles");
            dt_QueryArticles.Columns.Add("QueryArticleID");
            dt_QueryArticles.Columns.Add("QueryID");
            dt_QueryArticles.Columns.Add("PMID");
            dt_QueryArticles.PrimaryKey = new DataColumn[] { dt_QueryArticles.Columns["QueryArticleID"] };


            dt_KeywordList = new DataTable("T_KeyWordList");
            dt_KeywordList.Columns.Add("KeywordListID");
            dt_KeywordList.Columns.Add("KeywordOrder");
            dt_KeywordList.Columns.Add("KeywordID");
            dt_KeywordList.PrimaryKey = new DataColumn[] { dt_KeywordList.Columns["KeywordListID"]};


            dt_Keyword = new DataTable("T_Keyword");
            dt_Keyword.Columns.Add("KeywordID");
            dt_Keyword.Columns.Add("Keyword");
            dt_Keyword.PrimaryKey = new DataColumn[] { dt_Keyword.Columns["KeywordID"] };


            dt_Organism = new DataTable("T_Organism");
            dt_Organism.Columns.Add("OrganismID");
            dt_Organism.Columns.Add("OrganismName");
            dt_Organism.PrimaryKey = new DataColumn[] { dt_Organism.Columns["OrganismID"] };
             

            dt_QuerySession = new DataTable("T_QuerySession");
            dt_QuerySession.Columns.Add("QuerySessionID");
            dt_QuerySession.Columns.Add("QueryID");
            dt_QuerySession.Columns.Add("ProteinID");
            dt_QuerySession.Columns.Add("DateTime");
            dt_QuerySession.PrimaryKey = new DataColumn[] { dt_QuerySession.Columns["QuerySessionID"] };


            dt_Protein = new DataTable("T_Protein");
            dt_Protein.Columns.Add("ProteinID");
            dt_Protein.Columns.Add("ProteinName");
            dt_Protein.PrimaryKey = new DataColumn[] { dt_Protein.Columns["ProteinID"]};

            dt_ProteinList = new DataTable("ProteinList");
            dt_ProteinList.Columns.Add("ProteinListID");
            dt_ProteinList.Columns.Add("QuerySessionID");
            dt_ProteinList.Columns.Add("ProteinID");
            dt_ProteinList.PrimaryKey = new DataColumn[] { dt_ProteinList.Columns["ProteinListID"]};


            dt_AlternativeProteinName = new DataTable("T_AlternativeProteinName");
            dt_AlternativeProteinName.Columns.Add("AlternativeProteinID");
            dt_AlternativeProteinName.Columns.Add("AlternativeProteinName");
            dt_AlternativeProteinName.Columns.Add("ProteinID");
            dt_AlternativeProteinName.PrimaryKey = new DataColumn[] { dt_AlternativeProteinName.Columns["AlternativeProteinID"] };
                  
           
            dt_Article = new DataTable("T_Article");
            dt_Article.Columns.Add("PMID");
            dt_Article.Columns.Add("AuthorListID");
            dt_Article.Columns.Add("Title");
            dt_Article.Columns.Add("PubDate");
            dt_Article.Columns.Add("Language");
            dt_Article.Columns.Add("doi");
            dt_Article.Columns.Add("Abstract");
            dt_Article.Columns.Add("JournalRelease");
            dt_Article.Columns.Add("Pages");
            dt_Article.Columns.Add("Affiliation");
            dt_Article.Columns.Add("URL");
            dt_Article.PrimaryKey = new DataColumn[] { dt_Article.Columns["PMID"], 
                                                    dt_Article.Columns["AuthorListID"] };

        
            dt_AuthorList = new DataTable("T_AuthorList");
            dt_AuthorList.Columns.Add("PMID");
            dt_AuthorList.Columns.Add("AuthorListID");
            dt_AuthorList.Columns.Add("AuthorID");
            dt_AuthorList.Columns.Add("AuthorOrder", typeof(int));
            dt_AuthorList.PrimaryKey = new DataColumn[] { dt_AuthorList.Columns["PMID"], 
                                                        dt_AuthorList.Columns["AuthorListID"], 
                                                        dt_AuthorList.Columns["AuthorID"] };
           
            dt_Authors = new DataTable("T_Authors");
            dt_Authors.Columns.Add("AuthorID");
            dt_Authors.Columns.Add("Suffix");
            dt_Authors.Columns.Add("LastName");
            dt_Authors.Columns.Add("ForeName");
            dt_Authors.Columns.Add("Initials");
            dt_Authors.Columns.Add("CollectiveName"); 
            dt_Authors.PrimaryKey = new DataColumn[] { dt_Authors.Columns["AuthorID"] };
            

            dt_Journal_Release = new DataTable("T_Journal_Release");
            dt_Journal_Release.Columns.Add("JournalRelease");
            dt_Journal_Release.Columns.Add("JournalID");
            dt_Journal_Release.Columns.Add("Year");
            dt_Journal_Release.Columns.Add("Volume");
            dt_Journal_Release.Columns.Add("Issue");
            dt_Journal_Release.PrimaryKey = new DataColumn[] { dt_Journal_Release.Columns["JournalRelease"], 
                                                            dt_Journal_Release.Columns["JournalID"] };
           
            dt_Journal = new DataTable("T_Journals");
            dt_Journal.Columns.Add("JournalID");
            dt_Journal.Columns.Add("Title");
            dt_Journal.PrimaryKey = new DataColumn[] { dt_Journal.Columns["JournalID"] };
         

            dt_ArticleInfo = new DataTable("T_ArticleInfo");
            dt_ArticleInfo.Columns.Add("PMID");
            dt_ArticleInfo.Columns.Add("TagListID");
            dt_ArticleInfo.Columns.Add("CategoryListID");
            dt_ArticleInfo.Columns.Add("NotesID");
            dt_ArticleInfo.Columns.Add("URL");
            dt_ArticleInfo.PrimaryKey = new DataColumn[] { dt_ArticleInfo.Columns["PMID"], 
                                                        dt_ArticleInfo.Columns["TagListID"], 
                                                        dt_ArticleInfo.Columns["CategoryListID"], };

            dt_TagList = new DataTable("T_TagList");
            dt_TagList.Columns.Add("TagListID");
            dt_TagList.Columns.Add("TagID");
            dt_TagList.PrimaryKey = new DataColumn[] { dt_TagList.Columns["TagListID"]};


            dt_Tags = new DataTable("T_Tags");
            dt_Tags.Columns.Add("TagID");
            dt_Tags.Columns.Add("Tag");
            dt_Tags.PrimaryKey = new DataColumn[] { dt_Tags.Columns["TagID"] };


            dt_CategoryList = new DataTable("T_CategoryList");
            dt_CategoryList.Columns.Add("CategoryListID");
            dt_CategoryList.Columns.Add("CategoryID");
            dt_CategoryList.PrimaryKey = new DataColumn[] { dt_CategoryList.Columns["CategoryListID"]};


            dt_Categories = new DataTable("T_Categories");
            dt_Categories.Columns.Add("CategoryID");
            dt_Categories.Columns.Add("Category");
            dt_Categories.PrimaryKey = new DataColumn[] { dt_Categories.Columns["CategoryID"]};


            ds_Article = new DataSet("ArticleAuthorSet");


            Dict = new Dictionary<string,DataRow>();

        }

        // Name: getCout()
        // Parameters: one protein, one organism, and keywords list of string type 
        // Funtion: Retrieve number of articles and PMIDs and put them into the dataset.
        // Return: count number of int type
        override public int GetCount(string pro, string org, List<string>  l_Keys)
        {
            // retrieve all keywords' result at a time. 
            // but, need to consider what if retrieve one keyword at a time.??? 8/19

            #region Members
            string m_Pubmed_Search_Prefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils//esearch.fcgi?db=pubmed&retmax=5&organism=";
            #endregion

            // combine proteins and keywords
            string s_Key = string.Join(",", l_Keys);
            string s_Terms = string.Format("{0},{1}", pro, s_Key);

            // make url
            string s_AssembleURL = string.Format("{0}{1}&term={2}",
                                            m_Pubmed_Search_Prefix,
                                            org,
                                            s_Terms);

            // connect web
            WebClient client = new WebClient();
            string s_URL_Result = "";
            try
            {

                s_URL_Result = client.DownloadString(s_AssembleURL);

            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception caught while retrieving results from NCBI: " +
                    exc.ToString());
            }



            //Create the XmlDocument and load xml format strings.
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(s_URL_Result);

            //string s_Name = string.Format("{1},{2},{3}", s_Pro, s_Org, s_key);
            string s_Name = pro + org + s_Key;

            // T_Query
            DataRow rowQuery;
            if (Dict.TryGetValue(s_Name, out rowQuery))
            {
                // already exists
                return int.Parse(rowQuery["ResultCount"].ToString());
            }
            else
            {


                // T_Protein 
                DataRow rowPro;
                if (!(Dict.TryGetValue(pro, out rowPro)))
                {
                    rowPro = dt_Protein.NewRow();
                    rowPro["ProteinID"] = dt_Protein.Rows.Count + 1;
                    rowPro["ProteinName"] = pro;

                    dt_Protein.Rows.Add(rowPro);
                    Dict.Add(pro, rowPro);
                }

                // T_Organism
                DataRow rowOrg;
                if (!(Dict.TryGetValue(org, out rowOrg)))
                {
                    rowOrg = dt_Organism.NewRow();
                    rowOrg["OrganismID"] = dt_Organism.Rows.Count + 1;
                    rowOrg["OrganismName"] = org;

                    dt_Organism.Rows.Add(rowOrg);
                    Dict.Add(org, rowOrg);
                }

                
                // T_Keyword
                int i_keyOrder = 1;
                foreach (string keyword in l_Keys)
                {

                    DataRow rowKeyword;
                    if (!(Dict.TryGetValue(keyword, out rowKeyword)))
                    {
                        rowKeyword = dt_Keyword.NewRow();
                        rowKeyword["KeywordID"] = dt_Keyword.Rows.Count + 1;
                        rowKeyword["Keyword"] = keyword;

                        dt_Keyword.Rows.Add(rowKeyword);
                        Dict.Add(keyword, rowKeyword);

                    }


                    // T_KeywordList          
                    DataRow rowKeywordList = dt_KeywordList.NewRow();
                    rowKeywordList["KeywordListID"] = dt_Query.Rows.Count + 1;
                    rowKeywordList["KeywordOrder"] = i_keyOrder++;
                    rowKeywordList["KeywordID"] = rowKeyword["KeywordID"].ToString();
                    dt_KeywordList.Rows.Add(rowKeywordList);

                } // end of foreach for keyword and keywordlist

                rowQuery = dt_Query.NewRow();
                rowQuery["QueryID"] = dt_Query.Rows.Count + 1;
                rowQuery["Name"] = s_Name;
                //rowQuery["Description"] = ; ???
                rowQuery["ProteinID"] = rowPro["ProteinID"].ToString();
                rowQuery["OrganismID"] = rowOrg["OrganismID"].ToString();
                // keywordListID = QueryID because one query has one keywordList
                rowQuery["KeywordListID"] = rowQuery["QueryID"].ToString(); 
                rowQuery["ResultCount"] = doc.GetElementsByTagName("Count")[0].InnerText; // check!!!

                // Add a row of T_Query into the T_Query datatable and the dictionary
                dt_Query.Rows.Add(rowQuery);
                Dict.Add(s_Name, rowQuery);


            } // end of if else of T_Query
           
            
            // T_QueryArticles
            XmlNodeList PMIDs = doc.GetElementsByTagName("Id");
            foreach(XmlNode PMID in PMIDs)
            {
                DataRow rowQueryArticles = dt_QueryArticles.NewRow();
                rowQueryArticles["QueryArticleID"] = dt_QueryArticles.Rows.Count + 1;
                rowQueryArticles["QueryID"] = rowQuery["QueryID"];
                rowQueryArticles["PMID"] = PMID.InnerText ;
                IdList.Add(PMID.InnerText);
                dt_QueryArticles.Rows.Add(rowQueryArticles);

            }
     
            // T_QuerySession
            DataRow rowQuerySession = dt_QuerySession.NewRow();
            rowQuerySession["QuerySessionID"] = dt_QuerySession.Rows.Count + 1;
            rowQuerySession["QueryID"] = rowQuery["QueryID"];
            rowQuerySession["ProteinID"] = rowQuery["ProteinID"];
            rowQuerySession["DateTime"] = DateTime.Now;
            dt_QuerySession.Rows.Add(rowQuerySession);

            
            //T_ProteinList ???
            
            
            //T_AlternativeProteinName ???


            // add datatables in the dataset
            ds_Article.Tables.Add(dt_Query);
            ds_Article.Tables.Add(dt_Protein);
            ds_Article.Tables.Add(dt_Organism);
            ds_Article.Tables.Add(dt_Keyword);
            ds_Article.Tables.Add(dt_KeywordList);
            ds_Article.Tables.Add(dt_QueryArticles);
            ds_Article.Tables.Add(dt_QuerySession);
            //ds_Article.Tables.Add(dt_ProteinList);
            //ds_Article.Tables.Add(dt_AlternativeProteinName);


            return int.Parse(rowQuery["ResultCount"].ToString());

        }
        
        // Name: getArticleInfo()
        // Parameters: N/a
        // Function: Retrieve specific articles' information based on the PMIDs from the web-repositories
        //         and put those information into the dataset.
        // Return: null if it fails and a filled dataset if it successes.
        override public DataSet GetArticleInfo()
        {               
            #region Members
            string m_Pubmed_Search_Prefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=";
            #endregion

            string s_Ids = string.Join(",", IdList);
            
            // make url
            string s_AssembleURL = string.Format("{0}{1}&{2}", m_Pubmed_Search_Prefix, s_Ids, "retmode=xml");
         

            // connect web
            WebClient client = new WebClient();
            string s_URL_Result = "";
            try
            {
                s_URL_Result = client.DownloadString(s_AssembleURL);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception caught while retrieving results from NCBI: " +
                    exc.ToString());
            }

           XmlDocument doc = new XmlDocument();
           doc.LoadXml(s_URL_Result);
           ArticleList = doc.GetElementsByTagName("PubmedArticle");

           // fill the data tables from XMLNodeList
           foreach (XmlNode article in ArticleList)
           {

               /* T_Article */
               string pmid = "";
               pmid = article.SelectSingleNode("descendant::PMID").InnerText;

               // check article already exists. if yes, move to the next article
               if (Dict.ContainsKey(pmid))
                   continue;


               DataRow rowArticle = dt_Article.NewRow();
               rowArticle["PMID"] = pmid;
               rowArticle["AuthorListID"] = dt_Article.Rows.Count + 1;
               rowArticle["Title"] = article.SelectSingleNode("descendant::ArticleTitle").InnerText;
               rowArticle["PubDate"] = article.SelectSingleNode("descendant::ArticleDate") != null ?
                                   article.SelectSingleNode("descendant::ArticleDate").InnerText : null;
               rowArticle["Language"] = article.SelectSingleNode("descendant::Language") != null ?
                                   article.SelectSingleNode("descendant::Language").InnerText : null;
               rowArticle["doi"] = article.SelectSingleNode("descendant::ArticleIdList/ArticleId[@IdType='doi']") != null ?
                                   article.SelectSingleNode("descendant::ArticleIdList/ArticleId[@IdType='doi']").InnerText : null;
               rowArticle["Abstract"] = article.SelectSingleNode("descendant::Abstract") != null ?
                                   article.SelectSingleNode("descendant::Abstract").InnerText : null;
               // ??? What is journal release???
               rowArticle["JournalRelease"] = article.SelectSingleNode("descendant::ISSN") != null ?
                                   article.SelectSingleNode("descendant::ISSN").InnerText : null;
               rowArticle["Pages"] = article.SelectSingleNode("descendant::MedlinePgn") != null ?
                                   article.SelectSingleNode("descendant::MedlinePgn").InnerText : null;
               rowArticle["Affiliation"] = article.SelectSingleNode("descendant::Affiliation") != null ?
                                   article.SelectSingleNode("descendant::Affiliation").InnerText : null;
               string s_URL = string.Format("{0}{1}&{2}", m_Pubmed_Search_Prefix, rowArticle["PMID"].ToString(), "retmode=xml");
               rowArticle["URL"] = s_URL;

               // add a row into the T_Article and the dictionary
               dt_Article.Rows.Add(rowArticle);
               Dict.Add(pmid, rowArticle);


               /* T_Authors and T_AuthorList*/
               int i_AuthorOrder = 1;
               XmlNodeList authorList = article.SelectNodes("descendant::Author");

               foreach (XmlNode author in authorList)
               {


                   string s_authorName;
                   string lastName;
                   string foreName;
                   lastName = author.SelectSingleNode("descendant::LastName") != null ?
                           author.SelectSingleNode("descendant::LastName").InnerText : null;
                   foreName = author.SelectSingleNode("descendant::ForeName") != null ?
                           author.SelectSingleNode("descendant::ForeName").InnerText : null;
                   s_authorName = lastName + foreName;

                   // We currently do not count different persons with the same name.
                   // what if user restarts the application? Do I have to start authorID as 1 again?
                   // I may need to get author information (id and name) first from the database.
                   // chech the author already exists. If yes, move to the next the author
                   if (Dict.ContainsKey(s_authorName))
                       continue;


                   DataRow rowAuthor = dt_Authors.NewRow();
                   rowAuthor["AuthorID"] = dt_Authors.Rows.Count + 1;
                   rowAuthor["Suffix"] = author.SelectSingleNode("descendant::Suffix") != null ?
                                   author.SelectSingleNode("descendant::Suffix").InnerText : null;
                   rowAuthor["LastName"] = lastName;
                   rowAuthor["ForeName"] = foreName;
                   rowAuthor["Initials"] = author.SelectSingleNode("descendant::Initials") != null ?
                                   author.SelectSingleNode("descendant::Initials").InnerText : null;
                   rowAuthor["CollectiveName"] = author.SelectSingleNode("descendant::CollectiveName") != null ?
                                   author.SelectSingleNode("descendant::CollectiveName").InnerText : null;

                   // add the row into the T_Author and the dictionary
                   dt_Authors.Rows.Add(rowAuthor);
                   Dict.Add(s_authorName, rowAuthor);


                   // add a row into the T_AuthorList
                   DataRow rowAuthorList = dt_AuthorList.NewRow();
                   rowAuthorList["PMID"] = pmid;
                   rowAuthorList["AuthorListID"] = dt_Article.Rows.Count;
                   rowAuthorList["AuthorID"] = dt_Authors.Rows.Count;
                   rowAuthorList["AuthorOrder"] = i_AuthorOrder++;
                   dt_AuthorList.Rows.Add(rowAuthorList);


               } // end of inner foreach T_Authors and T_AuthorList



               // T_Journals
               string journalTitle = article.SelectSingleNode("descendant::Title") != null ?
                                   article.SelectSingleNode("descendant::Title").InnerText : null;
               DataRow rowJournals;

               // not exists, return false --> fill the datatable
               // exists, return true with rowJournal
               if (!(Dict.TryGetValue(journalTitle, out rowJournals)))
               {
                   rowJournals = dt_Journal.NewRow();
                   rowJournals["JournalID"] = dt_Journal.Rows.Count + 1;
                   rowJournals["Title"] = journalTitle;

                   dt_Journal.Rows.Add(rowJournals);
                   Dict.Add(journalTitle, rowJournals);
               }

               // T_JournalRelease
               string journalRelease;
               journalRelease = rowArticle["JournalRelease"].ToString();
               if (Dict.ContainsKey(journalRelease))
                   continue;

               DataRow rowJournal_Release = dt_Journal_Release.NewRow();
               rowJournal_Release["JournalRelease"] = journalRelease;
               rowJournal_Release["JournalID"] = rowJournals["JournalID"].ToString();
               rowJournal_Release["Year"] = article.SelectSingleNode("descendant::PubDate") != null ?
                                       article.SelectSingleNode("descendant::PubDate").InnerText : null;
               rowJournal_Release["Volume"] = article.SelectSingleNode("descendant::Volume") != null ?
                                       article.SelectSingleNode("descendant::Volume").InnerText : null;
               rowJournal_Release["Issue"] = article.SelectSingleNode("descendant::Issue") != null ?
                                       article.SelectSingleNode("descendant::Issue").InnerText : null;


               dt_Journal_Release.Rows.Add(rowJournal_Release);
               Dict.Add(rowJournal_Release["JournalRelease"].ToString(), rowJournal_Release);

           } // end of outer foreach


           // add the datatables into a dataset                                          
           ds_Article.Tables.Add(dt_Article);
           ds_Article.Tables.Add(dt_AuthorList);
           ds_Article.Tables.Add(dt_Authors);
           ds_Article.Tables.Add(dt_Journal);
           ds_Article.Tables.Add(dt_Journal_Release);

           // test that any data tables is empty in the dataset
           foreach (DataTable table in ds_Article.Tables)
           {
               if (table.Rows.Count == 0) // any table is empty in the dataset
                   return null;
           }

           return ds_Article;
        }
    }
}
