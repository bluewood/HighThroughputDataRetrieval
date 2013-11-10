using System;
using System.Collections.Generic;
using System.Data;
using HighThroughputDataRetrievalBackend.Util;
using NUnit.Framework;

namespace HighThroughputDataRetievalTests
{
    public class FunctionalTest
    {
        [Test]
        public void TestNcbiDataRetrieval()
        {
            // get current time 
            DateTime startDateTime = DateTime.Now;

            Console.WriteLine(@"Welcome to a simple PubMed Search...");


            const string proteinFromUser = "ips";
            const string organixmFromUser = "Human";
            const string keywordFromUser = "cell";

            // get protein, organism
            //while (string.IsNullOrEmpty(proteinFromUser) && string.IsNullOrEmpty(organixmFromUser))
            //{
            //    Console.WriteLine("Please enter the protein (e.g.: 'Gag') : ");
            //    proteinFromUser = Console.ReadLine().Trim();
            //    Console.WriteLine("Please enter the organism that you are interested in searching (e.g. 'Human'):");
            //    organixmFromUser = Console.ReadLine().Trim();

            //    if (proteinFromUser.Contains(' '))
            //        proteinFromUser = "";

            //    if (organixmFromUser.Contains(' '))
            //        organixmFromUser = ""; // organism cannot contain a space
            //}

            //// get terms
            ////List<string> l_keys = new List<string>();
            //while (string.IsNullOrEmpty(termsFromUser))
            //{
            //    Console.WriteLine("Please enter the terms to be searched with a space between them to delineate (e.g. 'HIV-1 Macrophage'):");
            //    termsFromUser = Console.ReadLine();
            //    string[] s_AllTerms = termsFromUser.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    l_keys = s_AllTerms.ToList();
            //}
            //string s_FinalTerms = string.Join(",", l_Terms);


            Console.WriteLine(@"Retrieving article information...Please wait");

            NcbiDataRetrieval test = new PubMedDataRetrieval();

            // get count and PMIDs
            //int count = test.GetCountAndIds("", "", "");
            int count = test.GetCount(proteinFromUser, organixmFromUser, keywordFromUser);
            List<string> idList = test.IdList;

            Console.WriteLine(@"count : " + count);

            //int lowBound = count/20;

            //for (int i = 0; i < lowBound; i++)
            //{
            //    test.GetArticleInfomation();
            //}
            
            DataTable articleDataTable = test.GetArticleInfomation(count, idList);
            //for (int i = 0; i < articleDataTable.Rows.Count; i++)
            //{
            //    if (idList[i] != articleDataTable.Rows[i]["PMID"].ToString())
            //    {
            //        Console.WriteLine(i + @" " + idList[i]);
            //        break;
            //    }
                
            //}
        

            //Console.WriteLine(@"Tables rows count: "+articleDataTable.Rows.Count);
            //articleDataTable.WriteXml("ArticleDataTable.xml");
            //if (articleDataSet != null)
            //{
            //    articleDataSet.WriteXml("Dataset.xml");
            //    Console.WriteLine(@"Retrieving article information successed!!");
            //    Console.WriteLine(@"Created Dataset.xml under bin/Debug folder. Please check inside.");
            //}
            //else
            //{
            //    Console.WriteLine(@"Retrieving article information failed!!");

            //}


            //data_base.create_data("C:/Users/Owner/Desktop/mydb.db3");
            //SQLiteConnection.CreateFile("C:/Users/Owner/Desktop/mydb.db3");
            //data_base.CopydatasetToDatabase("C:/Users/Owner/Desktop/mydb.db3", ds_Article);


            DataSet queryArticleDataSet = test.GetDataSet();
            queryArticleDataSet.WriteXml("check.xml");

            // get total running time
            TimeSpan runTime = DateTime.Now - startDateTime;
            Console.WriteLine(@"Completed in " + runTime.Hours + @" h " + runTime.Minutes + @" min + " + runTime.Seconds + @" sec.");
            //Console.ReadKey();
        }
    }
}
