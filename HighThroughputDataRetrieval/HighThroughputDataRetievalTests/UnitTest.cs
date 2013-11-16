using System.Collections.Generic;
using System.Data;
using HighThroughputDataRetrievalBackend.IO;
using NUnit.Framework;
using HighThroughputDataRetrievalBackend.Util;
using System.Net;
using System.Xml;


namespace HighThroughputDataRetievalTests
{

    public class UnitTest
    {
        // retrieval data unit testing
        #region Data members 

        string _proteinTest;
        string _organismTest;
        string _keywordTest;

        NcbiDataRetrieval _unitTestDataRetrieval;

        #endregion // Data members

        [SetUp]
        public void Init()
        {
            _proteinTest = "ips";
            _organismTest = "Human";
            _keywordTest = "cell";

            _unitTestDataRetrieval = new PubMedDataRetrieval();
        }


        // Database Unit testing
        #region Database unit test
        [Test]
        public void Test_1_Create_database()
        {
            //file is exist it must be false
            bool create = SqliteInputOutput.Create_database("D://Test/database.db3");
            //wrong input
            //bool create1 = SqliteInputOutput.Create_database(":/Test");
            Assert.AreEqual(false,create);
            //Assert.AreNotEqual(true,create1);

        }
        [Test]
        public void Test_2_CopydatasetToDatabase()

        {
            DataTable table1 = new DataTable("patients");
            table1.Columns.Add("name");
            table1.Columns.Add("id");
            table1.PrimaryKey = new[] { table1.Columns["id"] };
            table1.Columns.Add("address");
            table1.Rows.Add("Sam", 1, "pullman");
            table1.Rows.Add("Mark", 2, "SEATTLE");

            DataTable table2 = new DataTable("medications");
            table2.Columns.Add("id");
            table2.Columns.Add("medication");
            table2.Rows.Add(1, "atenolol");
            table2.Rows.Add(2, "amoxicillin");

            
            // Create a DataSet and put both tables in it.
            DataSet dataset = new DataSet("office");
            dataset.Tables.Add(table1);
            dataset.Tables.Add(table2);
            bool check = SqliteInputOutput.CopydatasetToDatabase("D://Test/database.db3", dataset);
            //wrong input
            
            Assert.AreEqual(true, check);

        }

        [Test]
        public void Test_3_AddTableToDataset()
        {
            DataTable table3 = new DataTable("staff");
            table3.Columns.Add("name");
            table3.Columns.Add("id");
            table3.Columns.Add("address");
            table3.Rows.Add("Perter", 1, "pullman");
            table3.Rows.Add("Christ", 2, "SEATTLE");
            bool check = SqliteInputOutput.AddTableToDataset("D://Test/database.db3",table3);
            Assert.AreEqual(true, check);
        }

        [Test]
        public void Test_4_GetTable()
        {
            DataTable myGetTable = SqliteInputOutput.GetTable("D://Test/database.db3", "staff");
            Assert.AreEqual("staff",myGetTable.TableName);
            Assert.AreEqual("name",myGetTable.Columns[0].ToString());
            Assert.AreEqual("id", myGetTable.Columns[1].ToString());
            Assert.AreEqual("address", myGetTable.Columns[2].ToString());
            // wrong input
            DataTable nullGetTable = SqliteInputOutput.GetTable("D://Test/database.db3", "abcd");
            Assert.AreEqual(null, nullGetTable);
           
        }
        [Test]
        public void Test_5_GetDataSet()
        {
            DataSet myDataSet = SqliteInputOutput.GetDataSet("D://Test/database.db3");
            Assert.AreEqual(3, myDataSet.Tables.Count);
          
            // wrong input will return empty dataset
            DataSet myDataSet1 = SqliteInputOutput.GetDataSet("D://Test/abc.db3");
            Assert.AreEqual(0, myDataSet1.Tables.Count);

        }
        [Test]
        public void Test_6_CreateIndex()
        {
            DataTable table4 = new DataTable("student");
            table4.Columns.Add("name");
            table4.Columns.Add("id");
            table4.Columns.Add("address");
            table4.Columns.Add("phone");
            table4.Rows.Add("Max", 1, "pullman",206-123-4567);
            table4.Rows.Add("Sarah", 2, "SEATTLE", 206-321-9999);
             // add new table.
            SqliteInputOutput.AddTableToDataset("D://Test/database.db3", table4);
            bool check = SqliteInputOutput.CreateIndex("D://Test/database.db3","student","name");
            Assert.AreEqual(true,check);
            //DataTable getTable = SqliteInputOutput.GetTable("D://Test/database.db3", "staff");
            
            // wrong input that doest not have table name "abc"
            bool check1 = SqliteInputOutput.CreateIndex("D://Test/database.db3", "abc", "abc");
            Assert.AreEqual(false, check1);
                   
         }

        [Test]
        public void Test_7_Search()
        {
            
            //DataTable holdTable = SqliteInputOutput.GetTable("D://Test/database.db3", "staff");
            DataTable table = SqliteInputOutput.Search("D://Test/database.db3", "staff", "name", "Peter");
            Assert.AreNotEqual(null,table);
            foreach (DataRow row in table.Rows)
            {
                Assert.AreEqual("Peter",row[0].ToString());
            }
           //wrong input
            DataTable table1 = SqliteInputOutput.Search("D://Test/database.db3", "staff", "name", "Lam");
// ReSharper disable once UnusedVariable
            foreach (DataRow row1 in table1.Rows)
            {
                Assert.AreEqual(null, table.Rows[0]);
            }

        }

        [Test]
        public void Test_8_CreateTableInDatasbase()
        {
            Dictionary<string, string> column = new Dictionary<string, string> {{"name", "TEXT"}, {"fee", "INTEGER"}};
            bool check = SqliteInputOutput.CreateTableInDatasbase("D://Test/database.db3", "Newtable", false, column);
            Assert.AreEqual(check, true);

        }

        [Test]
        public void Test_9_Insert()
        {
            Dictionary<string, string> value = new Dictionary<string, string> {{"Name", "Lam"}, {"fee", "1000"}};

            bool check = SqliteInputOutput.Insert("D://Test/database.db3", "Newtable", value);
            Assert.AreEqual(check, true);
        }

        [Test]
        public void Test_10_Update()
        {
            Dictionary<string, string> value = new Dictionary<string, string> { { "Name", "Nan" }};
            bool check = SqliteInputOutput.Update("D://Test/database.db3", "Newtable", value, "fee = 1000");
            Assert.AreEqual(check, true);
            //wrong input
            bool check1 = SqliteInputOutput.Update("D://Test/database.db3", "Newtable", value, "money = 11");
            Assert.AreEqual(false, check1);
        }

        [Test]
        public void Test_11_Delete()
        {
            bool check1 = SqliteInputOutput.Delete("D://Test/database.db3", "Newtable","fee=1000");
            Assert.AreEqual(true, check1);
        }


        #endregion // database unit test

        //Data Retrieval Unit testing 
        #region Data Retrieval Unit testing 
        [Test]
        public void TestGetCount()
        {
            // before run, double check there is new articles in pubmed !!!

            // count = 0
            Assert.AreEqual(0, _unitTestDataRetrieval.GetCount("", "", ""));

            // small count
            Assert.AreEqual(118, _unitTestDataRetrieval.GetCount("isp", "Human", "cell"));

            // large count
            Assert.AreEqual(238690, _unitTestDataRetrieval.GetCount("Hiv", "Human", ""));
            
        }

        [Test]
        public void TestFillQueryDatatables()
        {

            const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils//esearch.fcgi?db=pubmed&retmax=10000&";

            // small count
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
            _unitTestDataRetrieval.ArticleCount = int.Parse(xmlDocument.GetElementsByTagName("Count")[0].InnerText);
        
            // get PMIDs from xml string in the XmlNodeList
            XmlNodeList pmidListFromXml = xmlDocument.GetElementsByTagName("Id");

            Assert.AreEqual(true, _unitTestDataRetrieval.FillQueryDataTables(name, pmidListFromXml));


            //Assert.AreEqual(name, queryDataSet.Tables["T_Query"].Rows[0]["Name"].ToString());
            //Assert.AreEqual(count, queryDataSet.Tables["T_Query"].Rows[0]["ResultCount"]);
            //Assert.AreEqual(_proteinTest, queryDataSet.Tables["T_Protein"].Rows[0]["Protein"].ToString());
            //Assert.AreEqual(_organismTest, queryDataSet.Tables["T_Organism"].Rows[0]["Organism"].ToString());
            //Assert.AreEqual(_keywordTest, queryDataSet.Tables["T_Keyword"].Rows[0]["Keyword"].ToString());
            // Check T_Query

            // larger count

        }


       // [Test]

        //[Test]

        //public void TestGetArticleInfomation()
        //{
        //    //_proteinTest = "salmonella";
        //    //_organismTest = "";
        //    //_keywordTest = "Hiv-1";

        //    int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);
            
        //    int lowerBound = count/20;
        //    for (int i = 0; i < lowerBound; i++)
        //    {
        //        _unitTestDataRetrieval.GetArticleInfomation();
        //    }
        //    DataTable unitTestDataTable = _unitTestDataRetrieval.GetArticleInfomation();

            
        //    Console.WriteLine(@"count: " + count);
        //    Console.WriteLine(@"row count:" + unitTestDataTable.Rows.Count);
        //    Assert.AreEqual(count,unitTestDataTable.Rows.Count);
        //    //Assert.AreEqual(true, unitTestDataTable.Rows.Contains("Enteric Pathogens in HIV/AIDS from a Tertiary Care Hospital."));
        //    Assert.AreEqual("Future horizons in hair restoration.", unitTestDataTable.Rows[8]["Title"].ToString());


        //}

        //[Test]
        //public void TestFillArticleDataTables()
        //{
        //    const string pubmedSearchPrefix = "http://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi?db=pubmed&id=";
           
        //    int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);

        //    int lowBound = count/20;
        //    int retrievedArticleCount = 0;
        //    int numberOfRetrieving = 20;

        //    List<string> idList = _unitTestDataRetrieval.IdList;

        //    for (int i = 0; i < lowBound; i++)
        //    {
        //        if (retrievedArticleCount == count)
        //            break;

        //        // the Url is too long when retrieving articles over 100 at a time, so occur exception
        //        // so retrieving 20 articles at a time when user click more
               

        //        string ids = string.Join(",",idList.GetRange(retrievedArticleCount, numberOfRetrieving));
        //        retrievedArticleCount += numberOfRetrieving;

        //        // make url
        //        string assembleUrl = string.Format("{0}{1}&{2}", pubmedSearchPrefix, ids, "retmode=xml");


        //        // Retrieve article information from PubMed through the URL
        //        var client = new WebClient();
        //        string urlResult = "";
        //        try
        //        {
        //            urlResult = client.DownloadString(assembleUrl);
        //        }
        //        catch (Exception exc)
        //        {
        //            Console.WriteLine(exc.Message);
        //        }


        //        // load result in xmlformat and parse per article
        //        var doc = new XmlDocument();
        //        doc.LoadXml(urlResult);
        //        XmlNodeList articleListFromXml = doc.GetElementsByTagName("PubmedArticle");

        //        _unitTestDataRetrieval.FillArticleDataTables(articleListFromXml, pubmedSearchPrefix);
          
        //    } // end of for
         

        //    // the Url is too long when retrieving articles over 100 at a time, so occur exception
        //    // so retrieving 20 articles at a time when user click more
            
        //    if ((count - retrievedArticleCount) < numberOfRetrieving)
        //        numberOfRetrieving = count - retrievedArticleCount;

        //    string idsLast = string.Join(",", idList.GetRange(retrievedArticleCount, numberOfRetrieving));
        //    retrievedArticleCount += numberOfRetrieving;

        //    // make url
        //    string assembleUrlLast = string.Format("{0}{1}&{2}", pubmedSearchPrefix, idsLast, "retmode=xml");


        //    // Retrieve article information from PubMed through the URL
        //    var clientLast = new WebClient();
        //    string urlResultLast = "";
        //    try
        //    {
        //        urlResultLast = clientLast.DownloadString(assembleUrlLast);
        //    }
        //    catch (Exception exc)
        //    {
        //        Console.WriteLine(exc.Message);
        //    }


        //    // load result in xmlformat and parse per article
        //    var docLast = new XmlDocument();
        //    docLast.LoadXml(urlResultLast);
        //    XmlNodeList articleListFromXmlLast = docLast.GetElementsByTagName("PubmedArticle");
        //    _unitTestDataRetrieval.FillArticleDataTables(articleListFromXmlLast, pubmedSearchPrefix);

        //    DataSet articleDataSet = _unitTestDataRetrieval.GetDataSet();

        //    string articleTitle = "Functional analysis of serially expanded human iPS cell-derived RPE cultures.";

        //    // check T_Article
        //    //Assert.AreEqual(articleTitle, articleDataT);
        //}

        //[Test]
        //public void TestGetDataSet()
        //{
        //    _proteinTest = "ips";
        //    _organismTest = "Human";
        //    _keywordTest = "cell";

        //    string name = _proteinTest + _organismTest + _keywordTest;

        //    int count = _unitTestDataRetrieval.GetCount(_proteinTest, _organismTest, _keywordTest);
            

        //    int lowBound = count/20;
        //    for (int i = 0; i < lowBound; i++)
        //    {
        //        _unitTestDataRetrieval.GetArticleInfomation();
        //    }
        //    _unitTestDataRetrieval.GetArticleInfomation();

        //    DataSet queryArticleDataSet = _unitTestDataRetrieval.GetDataSet();

        //    // check Query data table
        //    Assert.AreEqual(name, queryArticleDataSet.Tables["T_Query"].Rows[0]["Name"]);
        //    Assert.AreEqual(count, queryArticleDataSet.Tables["T_Query"].Rows[0]["ResultCount"]);

        //    // check Protein, Organism, Keyword data table
        //    Assert.AreEqual(_proteinTest, queryArticleDataSet.Tables["T_Protein"].Rows[0]["Protein"]);
        //    Assert.AreEqual(_organismTest, queryArticleDataSet.Tables["T_Organism"].Rows[0]["Organism"]);
        //    Assert.AreEqual(_keywordTest, queryArticleDataSet.Tables["T_Keyword"].Rows[0]["Keyword"]);

        //    // check Article data table
        //    Assert.AreEqual(count, queryArticleDataSet.Tables["T_Article"].Rows.Count);
        //    Assert.AreEqual("iPS cells in humans.", queryArticleDataSet.Tables["T_Article"].Rows[6]["Title"]);

        //   // check Author data table
        //    Assert.AreEqual("Cyranoski", queryArticleDataSet.Tables["T_Authors"].Rows[0]["LastName"]);

        //    // check Journal data table
        //    Assert.AreEqual("Nature biotechnology", queryArticleDataSet.Tables["T_Journals"].Rows[0]["Title"]);

        //    // check Journal Release data table
        //    Assert.AreEqual("1546-1696", queryArticleDataSet.Tables["T_JournalRelease"].Rows[0]["JournalRelease"]);
        //}

        #endregion

    }
}
