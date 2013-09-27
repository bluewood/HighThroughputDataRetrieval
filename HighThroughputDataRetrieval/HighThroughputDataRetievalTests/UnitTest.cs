using System;
using System.Collections.Generic;
using System.Data;
using HighThroughputDataRetrievalBackend.Util;
using NUnit.Framework;
using System.Net;
using System.Xml;

namespace HighThroughputDataRetievalTests
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class UnitTest
    {
        string _proteinTest;
        string _organismTest;
        string _keywordTest;

        NcbiDataRetrieval _unitTestDataRetrieval;

        [SetUp]
        public void Init()
        {
            _proteinTest = "ips";
            _organismTest = "Human";
            _keywordTest = "cell";

            _unitTestDataRetrieval = new PubMedDataRetrieval();
        }

        [Test]
        public void TestGetCountAndIds()
        {
            // before run, double check there is new articles in pubmed
            Assert.AreEqual(1553, _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest));
            Assert.AreEqual(0, _unitTestDataRetrieval.GetCount("", "", ""));
        }

        [Test]
        public void TestFillQueryDatatables()
        {

            const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils//esearch.fcgi?db=pubmed&retmax=10000&";

            // combine protein, organism, and keyword and construct the URL
            string terms = string.Format("{0},{1},{2}", _proteinTest, _organismTest, _keywordTest);           
            string assembleUrl = string.Format("{0}&term={1}", pubmedSearchPrefix, terms);

            // connect pubmed and retrieve the information that are count number and PMID list.
            var client = new WebClient();
            string urlResult = client.DownloadString(assembleUrl);

            //Create the XmlDocument and load the resut as xml format strings.
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(urlResult);

            // name is the name of T_Query table and needed to check duplication query
            string name = _proteinTest + _organismTest + _keywordTest;

            // get count from xml string
            int count = int.Parse(xmlDocument.GetElementsByTagName("Count")[0].InnerText);
        
            // get PMIDs from xml string in the XmlNodeList
            XmlNodeList pmidListFromXml = xmlDocument.GetElementsByTagName("Id");

            _unitTestDataRetrieval.FillQueryDataTables(name, count, pmidListFromXml);

            DataSet queryDataSet = _unitTestDataRetrieval.GetDataSet();
            queryDataSet.Tables["T_Query"].WriteXml("queryTable.xml");
            //Console.WriteLine(((queryDataSet.Tables["T_Query"]).Rows[0]["Name"]).ToString());

            //Assert.AreEqual(name, queryDataSet.Tables["T_Query"].Rows[0]["Name"].ToString());
            //Assert.AreEqual(count, queryDataSet.Tables["T_Query"].Rows[0]["ResultCount"]);
            //Assert.AreEqual(_proteinTest, queryDataSet.Tables["T_Protein"].Rows[0]["Protein"].ToString());
            //Assert.AreEqual(_organismTest, queryDataSet.Tables["T_Organism"].Rows[0]["Organism"].ToString());
            //Assert.AreEqual(_keywordTest, queryDataSet.Tables["T_Keyword"].Rows[0]["Keyword"].ToString());
            // Check T_Query

        }

        [Test]
        public void TestGetArticleInfomation()
        {
            //_proteinTest = "salmonella";
            //_organismTest = "";
            //_keywordTest = "Hiv-1";

            int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);
            
            int lowerBound = count/20;
            for (int i = 0; i < lowerBound; i++)
            {
                _unitTestDataRetrieval.GetArticleInfomation();
            }
            DataTable unitTestDataTable = _unitTestDataRetrieval.GetArticleInfomation();

            
            Console.WriteLine(@"count: " + count);
            Console.WriteLine(@"row count:" + unitTestDataTable.Rows.Count);
            Assert.AreEqual(count,unitTestDataTable.Rows.Count);
            //Assert.AreEqual(true, unitTestDataTable.Rows.Contains("Enteric Pathogens in HIV/AIDS from a Tertiary Care Hospital."));
            Assert.AreEqual("Future horizons in hair restoration.", unitTestDataTable.Rows[8]["Title"].ToString());


        }

        [Test]
        public void TestFillArticleDataTables()
        {
            const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=";
           
            int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);

            int lowBound = count/20;
            int retrievedArticleCount = 0;
            int numberOfRetrieving = 20;

            List<string> idList = _unitTestDataRetrieval.GetIdList();

            for (int i = 0; i < lowBound; i++)
            {
                if (retrievedArticleCount == count)
                    break;

                // the Url is too long when retrieving articles over 100 at a time, so occur exception
                // so retrieving 20 articles at a time when user click more
               

                string ids = string.Join(",",idList.GetRange(retrievedArticleCount, numberOfRetrieving));
                retrievedArticleCount += numberOfRetrieving;

                // make url
                string assembleUrl = string.Format("{0}{1}&{2}", pubmedSearchPrefix, ids, "retmode=xml");


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

                _unitTestDataRetrieval.FillArticleDataTables(articleListFromXml, pubmedSearchPrefix);
          
            } // end of for
         

            // the Url is too long when retrieving articles over 100 at a time, so occur exception
            // so retrieving 20 articles at a time when user click more
            
            if ((count - retrievedArticleCount) < numberOfRetrieving)
                numberOfRetrieving = count - retrievedArticleCount;

            string idsLast = string.Join(",", idList.GetRange(retrievedArticleCount, numberOfRetrieving));
            retrievedArticleCount += numberOfRetrieving;

            // make url
            string assembleUrlLast = string.Format("{0}{1}&{2}", pubmedSearchPrefix, idsLast, "retmode=xml");


            // Retrieve article information from PubMed through the URL
            var clientLast = new WebClient();
            string urlResultLast = "";
            try
            {
                urlResultLast = clientLast.DownloadString(assembleUrlLast);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }


            // load result in xmlformat and parse per article
            var docLast = new XmlDocument();
            docLast.LoadXml(urlResultLast);
            XmlNodeList articleListFromXmlLast = docLast.GetElementsByTagName("PubmedArticle");
            _unitTestDataRetrieval.FillArticleDataTables(articleListFromXmlLast, pubmedSearchPrefix);

            DataSet articleDataSet = _unitTestDataRetrieval.GetDataSet();

            string articleTitle = "Functional analysis of serially expanded human iPS cell-derived RPE cultures.";

            // check T_Article
            //Assert.AreEqual(articleTitle, articleDataT);
        }

        [Test]
        public void TestGetDataSet()
        {
            _proteinTest = "ips";
            _organismTest = "Human";
            _keywordTest = "cell";

            string name = _proteinTest + _organismTest + _keywordTest;

            int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);
            

            int lowBound = count/20;
            for (int i = 0; i < lowBound; i++)
            {
                _unitTestDataRetrieval.GetArticleInfomation();
            }
            _unitTestDataRetrieval.GetArticleInfomation();

            DataSet queryArticleDataSet = _unitTestDataRetrieval.GetDataSet();

            // check Query data table
            Assert.AreEqual(name, queryArticleDataSet.Tables["T_Query"].Rows[0]["Name"]);
            Assert.AreEqual(count, queryArticleDataSet.Tables["T_Query"].Rows[0]["ResultCount"]);

            // check Protein, Organism, Keyword data table
            Assert.AreEqual(_proteinTest, queryArticleDataSet.Tables["T_Protein"].Rows[0]["Protein"]);
            Assert.AreEqual(_organismTest, queryArticleDataSet.Tables["T_Organism"].Rows[0]["Organism"]);
            Assert.AreEqual(_keywordTest, queryArticleDataSet.Tables["T_Keyword"].Rows[0]["Keyword"]);

            // check Article data table
            Assert.AreEqual(count, queryArticleDataSet.Tables["T_Article"].Rows.Count);
            Assert.AreEqual("iPS cells in humans.", queryArticleDataSet.Tables["T_Article"].Rows[6]["Title"]);

           // check Author data table
            Assert.AreEqual("Cyranoski", queryArticleDataSet.Tables["T_Authors"].Rows[0]["LastName"]);

            // check Journal data table
            Assert.AreEqual("Nature biotechnology", queryArticleDataSet.Tables["T_Journals"].Rows[0]["Title"]);

            // check Journal Release data table
            Assert.AreEqual("1546-1696", queryArticleDataSet.Tables["T_JournalRelease"].Rows[0]["JournalRelease"]);
        }



    }
}
