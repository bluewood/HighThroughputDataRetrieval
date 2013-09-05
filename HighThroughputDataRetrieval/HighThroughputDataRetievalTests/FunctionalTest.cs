using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
            DateTime dt_Start = DateTime.Now;

            Console.WriteLine("Welcome to a simple PubMed Search...\n");

            string s_Org = "", s_Terms = "", s_Pro = "";

            // get organism
            while (string.IsNullOrEmpty(s_Pro) && string.IsNullOrEmpty(s_Org))
            {
                Console.WriteLine("Please enter the protein (e.g.: 'Gag') : ");
                s_Pro = Console.ReadLine().Trim();
                Console.WriteLine("Please enter the organism that you are interested in searching (e.g. 'Human'):");
                s_Org = Console.ReadLine().Trim();

                if (s_Pro.Contains(' '))
                    s_Pro = "";

                if (s_Org.Contains(' '))
                    s_Org = ""; // organism cannot contain a space
            }

            // get terms
            List<string> l_keys = new List<string>();
            while (string.IsNullOrEmpty(s_Terms))
            {
                Console.WriteLine("Please enter the terms to be searched with a space between them to delineate (e.g. 'HIV-1 Macrophage'):");
                s_Terms = Console.ReadLine();
                string[] s_AllTerms = s_Terms.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                l_keys = s_AllTerms.ToList();
            }
            //string s_FinalTerms = string.Join(",", l_Terms);


            Console.WriteLine("\n" + "Started to retrieve article information...Please wait");

            NcbiDataRetrieval test = new PubMedDataRetrieval();

            // get count and PMIDs
            int count = test.GetCount(s_Pro, s_Org, l_keys);

            Console.WriteLine("count : " + count);


            // get dataset. need to modify the maximum number (retmax) of articles
            DataSet ds_Article = test.GetArticleInfo();
            if (ds_Article != null)
            {
                ds_Article.WriteXml("Dataset.xml");
                Console.WriteLine("Retrieving article information successed!!");
                Console.WriteLine("Created Dataset.xml under bin/Debug folder. Please check inside.");
            }
            else
            {
                Console.WriteLine("Retrieving article information failed!!");

            }


            //data_base.create_data("C:/Users/Owner/Desktop/mydb.db3");
            //SQLiteConnection.CreateFile("C:/Users/Owner/Desktop/mydb.db3");
            //data_base.CopydatasetToDatabase("C:/Users/Owner/Desktop/mydb.db3", ds_Article);




            // get total running time
            TimeSpan RunTime = DateTime.Now - dt_Start;
            Console.WriteLine("Completed in " + RunTime.Hours + " h " + RunTime.Minutes + " min + " + RunTime.Seconds + " sec.");
            Console.ReadKey();
        }
    }
}
