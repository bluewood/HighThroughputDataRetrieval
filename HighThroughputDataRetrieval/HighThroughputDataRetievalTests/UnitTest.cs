using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using HighThroughputDataRetrievalBackend.IO;
using NUnit.Framework;


namespace HighThroughputDataRetievalTests
{
    public class UnitTest
    {
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
    }
}
