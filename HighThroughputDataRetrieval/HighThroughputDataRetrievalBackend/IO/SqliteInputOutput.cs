using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
//using System.IO;

namespace HighThroughputDataRetrievalBackend.IO
{
    public class SqliteInputOutput
    {
        //conver from DataColumn to String Name
        private static string ConverType(string dataType)
        {
            string type = "";
            switch (dataType)
            {
                case "System.String":
                    type = string.Format("TEXT");
                    break;
                case "System.Int16":
                    type = string.Format("INTEGER");
                    break;
                case "System.Int32":
                    type = string.Format("INTEGER");
                    break;
                case "System.Int64":
                    type = string.Format("INTEGER");
                    break;
                case "System.Double":
                    type = string.Format("DOUBLE");
                    break;
            }
            return type;
        }
        /// <summary>
        /// fill the value of dataset to datatable.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="data"></param>

        private static void Fill(SQLiteConnection connection, DataSet data)
        {

            foreach (DataTable table in data.Tables)
            {
                using (SQLiteTransaction dbTrans = connection.BeginTransaction())
                {
                    using (SQLiteCommand cmd = connection.CreateCommand())
                    {
                        List<string> listCol = new List<string>();
// ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (DataColumn dc in table.Columns)
                            listCol.Add(dc.ColumnName);

                        cmd.CommandText = string.Format(
                            "INSERT INTO {0}({1}) VALUES (@{2});", table.TableName,
                            string.Join(", ", listCol),
                            string.Join(", @", listCol));

                        //Console.WriteLine("Insert Statement:\n" +cmd.CommandText);

                        // ReSharper disable once UnusedVariable
                        foreach (string s in listCol)
                        {
                            SQLiteParameter param = cmd.CreateParameter();
                            cmd.Parameters.Add(param);
                        }

                        foreach (DataRow dr in table.Rows)
                        {
                            int idx = 0;
                            foreach (SQLiteParameter p in cmd.Parameters)
                            {
                                p.ParameterName = "@" + listCol[idx];
                                p.SourceColumn = listCol[idx];
                                p.Value = dr[idx];
                                idx++;
                            }

                            cmd.ExecuteNonQuery();
                        }
                    }

                    dbTrans.Commit();
                }
            }
        }

        /// <summary>
        /// create the emty db file. If the filename is existed then return false
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        public static bool Create_database(string filename)
        {

            bool check;
            try
            {
                SQLiteConnection conn = new SQLiteConnection("Data Source=" + filename + "; FailIfMissing=True");
                conn.Open();
                conn.Close();
                check = true; // true mean the db is existed
            }
            catch (Exception)
            {
                check = false; // db not exist
            }
            if ((filename != null) && (check == false))
            {
                SQLiteConnection.CreateFile(filename);
                return true;
            }
            return false;

        }

        //
        /// <summary>
        ///  copy dataset to database. that will create table with the collums abd then call function fill 
        /// to fill dataset to data table.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <returns>bool</returns>
        public static bool CopydatasetToDatabase(string filename, DataSet data)
        {
            bool bReturn = true;
            SQLiteConnection conn = new SQLiteConnection(
                    "Data Source=" + filename);
            try
            {
                conn.Open();

                foreach (DataTable dt in data.Tables)
                {
                    //string sCmd1 = string.Format("DROP TABLE IF EXISTS {0};", dt.TableName);
                    //SQLiteCommand cmd1 = new SQLiteCommand(sCmd1, conn);
                    //cmd1.ExecuteNonQuery();

                    string sCmd2 = string.Format("CREATE TABLE IF NOT EXISTS {0} (", dt.TableName);
                    // get the collum name and type 
                    List<string> listOfColum = new List<string>();
                    List<string> listOfPk = new List<string>();
                    DataColumn[] col = dt.PrimaryKey;
// ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (DataColumn dc in dt.Columns)
                    {
                        string colum = dc.ColumnName + " ";
                        colum += ConverType(dc.DataType.FullName);
                        listOfColum.Add(colum);
                    }
                    sCmd2 += string.Join(", ", listOfColum);
                    if (col.Length > 0)
                    {
                        sCmd2 += ", ";
                        sCmd2 += "PRIMARY KEY (";
// ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (DataColumn c in col)
                        {
                            string pk = string.Join(", ", c.ColumnName);
                            listOfPk.Add(pk);
                        }
                        sCmd2 += string.Join(", ", listOfPk);
                        sCmd2 += ")";
                    }
                    sCmd2 += ");";

                    SQLiteCommand cmd1 = new SQLiteCommand(sCmd2, conn);
                    Console.WriteLine(sCmd2);
                    cmd1.ExecuteNonQuery();
                }

                Fill(conn, data);
            }
            catch (Exception)
            {
                //Console.WriteLine("Error in CopyToDatabase:\n" +exc.ToString());
                bReturn = false;
            }
            finally
            {
                conn.Close();
            }

            return bReturn;
        }

        /// <summary>
        /// write data table to database
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="table"></param>
        /// <returns></returns>

        public static bool AddTableToDataset(string filename, DataTable table)
        {
            bool bReturn = true;
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + filename);
            try
            {
                conn.Open();
                string scmd = string.Format("DROP TABLE IF EXISTS {0};", table.TableName);
                SQLiteCommand Cmd = new SQLiteCommand(scmd, conn);
                Cmd.ExecuteNonQuery();

                string scmd1 = string.Format("CREATE TABLE {0} (", table.TableName);
                // get the collum name and type 
                List<string> listOfColum = new List<string>();
                List<string> listOfPk = new List<string>();
                DataColumn[] col = table.PrimaryKey;
// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (DataColumn dc in table.Columns)
                {
                    string colum = dc.ColumnName + " ";
                    colum += ConverType(dc.DataType.FullName);
                    listOfColum.Add(colum);
                }
                scmd1 += string.Join(", ", listOfColum);
                //write the primary key
                if (col.Length > 0)
                {
                    scmd1 += ",";
                    scmd1 += "PRIMARY KEY (";
// ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (DataColumn c in col)
                    {
                        string pk = string.Join(", ", c.ColumnName);
                        listOfPk.Add(pk);
                    }
                    scmd1 += string.Join(", ", listOfPk);
                    scmd1 += ")";
                }
                scmd1 += ");";
                Cmd = new SQLiteCommand(scmd1, conn);
                Cmd.ExecuteNonQuery();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    List<string> l_Col = new List<string>();
                    foreach (DataColumn dc in table.Columns)
                        l_Col.Add(dc.ColumnName);

                    cmd.CommandText = string.Format(
                        "INSERT INTO {0}({1}) VALUES (@{2});",
                        table.TableName,
                        string.Join(", ", l_Col),
                        string.Join(", @", l_Col));

                    //Console.WriteLine("Insert Statement:\n" + cmd.CommandText);

                    foreach (string s in l_Col)
                    {
                        SQLiteParameter param = cmd.CreateParameter();
                        cmd.Parameters.Add(param);
                    }

                    foreach (DataRow dr in table.Rows)
                    {
                        int idx = 0;
                        foreach (SQLiteParameter p in cmd.Parameters)
                        {
                            p.ParameterName = "@" + l_Col[idx];
                            p.SourceColumn = l_Col[idx];
                            p.Value = dr[idx];
                            idx++;
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
               // Console.WriteLine(@"Error in CopyToDatabase:" + exc.ToString());
                bReturn = false;
            }
            conn.Close();
            return bReturn;

        }
        /// <summary>
        /// Return Table inside database by the table name.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable GetTable(string filename, string tableName)
        {
            DataTable table = new DataTable(tableName);
            string command;
            if(tableName!="")
                command = string.Format("SELECT * FROM {0};", tableName);
            else
            {
                //table = null;
                return null;
            }

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(command, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    table.Load(reader);
                    conn.Close();
                }

            }

            catch (Exception)
            {
                return null;
                // Console.WriteLine("Exception in GetTable(): " +exc.ToString());
            }
            return table;
        }
        /// <summary>
        /// Get dataSet from database
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string filename)
        {

            DataSet ds = new DataSet();
            List<string> getTables = new List<string>();
            const string command = "SELECT * FROM sqlite_master WHERE type='table'";

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(command, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    conn.Close();
                    // get list of table
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (DataRow dr in dt.Rows)
                    {
                        getTables.Add(dr["tbl_name"].ToString());
                    }
                }
            }
            catch (Exception)
            {
               return ds;
            }
            //read and get table in list

            foreach (string tb in getTables)
            {
                DataTable table = new DataTable(tb);
                string command1 = string.Format("SELECT * FROM {0};", tb);
                try
                {
                    var connStr = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = filename
                    };

                    using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                    {
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand(command1, conn);
                        SQLiteDataReader reader1 = cmd.ExecuteReader();
                        table.Load(reader1);
                        conn.Close();
                    }

                }
                catch (Exception)
                {
                  return ds;
                }
                ds.Tables.Add(table);

            }

            return ds;
        }

        // Create Index for table name and colum
        
        public static bool CreateIndex(string filename, string tableName, string columnName)
        {
            //bool rt = true;
            try
            {
                string index = string.Format("idex" + columnName);
                string command = string.Format("CREATE INDEX {0} ON {1}({2});", index, tableName, columnName);
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(command, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            catch (Exception)
            {
                //Console.WriteLine("Exception in CreateIndex(): " + exc.ToString());
                return false;

            }

            return true;
        }
        /// <summary>
        /// function will do the sql statement on the database
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        public static bool RunSelectQuery(string filename, string sqlStatement)
        {
            bool rt = true;
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(sqlStatement, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception)
            {
               // Console.WriteLine("Exception in RunSelectQuery: " + exc.ToString());
                rt = false;
            }
            return rt;
        }
        //Create the database table
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ttableName"></param>
        /// <param name="where"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DataTable Search(string connection, string ttableName, string where, string key)
        {
            DataTable result = new DataTable("Result");
            String command = string.Format("SELECT * FROM {0} WHERE {1} = '{2}' ", ttableName, where, key);
            Console.WriteLine(command);
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(command, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    result.Load(reader);
                    conn.Close();
                }

            }

            catch (Exception)
            {
                return null;
                //Console.WriteLine("Exception in Search(): " + exc.ToString());
            }
            return result;

        }
        public static bool CreateTableInDatasbase(string filename, string tableName, bool autoId,
             Dictionary<string, string> columns)
        {
            string stCommand = "CREATE TABLE " + tableName + "(" +
                (autoId ? "ID INTEGER PRIMARY KEY AUTOINCREMENT, " : "");

// ReSharper disable once LoopCanBeConvertedToQuery
            foreach (string k in columns.Keys)
            {
                stCommand += k + " " + columns[k] + ", ";
            }
            stCommand = stCommand.Substring(0, stCommand.Length - 2);   // removes the last comma and space
            stCommand += ");";

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn) {CommandText = stCommand};
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception)
            {
                return false;
                // Console.WriteLine(@"SQLite Handler IOException in CreateTable(): " + ioe.ToString());
            }
            return true;
            
        }



        // Insert in to table
        public static bool Insert(string filename, string tableName, Dictionary<string, string> data)
        {
            string columns = "";
            string values = "";
            bool returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key);
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            string myInsertQuery = string.Format("INSERT INTO {0}({1}) VALUES ({2});", tableName, columns, values);
            Console.WriteLine(myInsertQuery);
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn) {CommandText = myInsertQuery};
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            catch (Exception)
            {
                //System.Console.WriteLine(fail.Message);
                returnCode = false;
            }

            return returnCode;
        }


        //update table       
        public static bool Update(string filename, string tableName, Dictionary<string, string> data, string where)
        {
            string vals = "";
            bool returnCode = true;
            if (data.Count >= 1)
            {
// ReSharper disable once LoopCanBeConvertedToQuery
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(CultureInfo.InvariantCulture), val.Value.ToString(CultureInfo.InvariantCulture));
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            string myUpdateQuery = string.Format("UPDATE {0} SET {1} WHERE {2};", tableName, vals, where);

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn) {CommandText = myUpdateQuery};
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            catch (Exception)
            {
                //System.Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        //delete from table
        public static bool Delete(string filename, string tableName, string where)
        {
            bool returnCode = true;
            string myDeleteQuery = string.Format("DELETE FROM {0} WHERE {1};", tableName, where);
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = filename
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn) {CommandText = myDeleteQuery};
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception)
            {
                //System.Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }
    }
}
