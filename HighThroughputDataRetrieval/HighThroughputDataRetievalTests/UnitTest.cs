using System.Data;
using HighThroughputDataRetrievalBackend.Util;
using NUnit.Framework;
using System.Net;
using System.Xml;

namespace HighThroughputDataRetievalTests
{
    public class UnitTest
    {
        string _proteinTest;
        string _organismTest;
        string _keywordTest;

        NcbiDataRetrieval _unitTestDataRetrieval;

        [SetUp]
        public void Init()
        {
            _proteinTest = "Gag";
            _organismTest = "Human";
            _keywordTest = "HIV-1";

            _unitTestDataRetrieval = new PubMedDataRetrieval();
        }



        [Test]
        public void GetCountAndIds()
        {

            Assert.AreEqual(5565, _unitTestDataRetrieval.GetCountAndIds(_proteinTest, _organismTest, _keywordTest));

            // call again to check if checking duplication works.
            Assert.AreEqual(5565, _unitTestDataRetrieval.GetCountAndIds(_proteinTest, _organismTest, _keywordTest));
        }
        [Test]
        public void FillQueryDatatables()
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

            _unitTestDataRetrieval.FillQueryDatatables(name, count, pmidListFromXml);

            //Assert.AreEqual(name, _unitTestDataRetrieval.);
            // can I get a paper?

        }

        [Test]
        public void GetArticleInfomation()
        {
            int count = _unitTestDataRetrieval.GetCountAndIds(_proteinTest, _organismTest, _keywordTest);

            int lowerBound = count/20;
            for (int i = 0; i < lowerBound; i++)
            {
                _unitTestDataRetrieval.GetArticleInfomation();
            }
            DataTable unitTestDataTable = _unitTestDataRetrieval.GetArticleInfomation();

            // check two representive datatables, T_Query and T_Article, 
            // from query part and article information part
            //DataTable unitTestQuery = unitTestDataTable.Tables["T_Query"];
            //DataTable unitTestArticle = unitTestDataTable.Tables["T_Article"];
            
            //Assert.AreEqual(count, unitTestDataTable.Rows.Count);


        }

        //public void FillArticleDatatables()
        //{


        //}

    }
}
