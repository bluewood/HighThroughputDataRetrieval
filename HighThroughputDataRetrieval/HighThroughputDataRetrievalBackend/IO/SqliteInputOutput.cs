using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

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
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>

        private static void Fill(SQLiteConnection conn, DataSet data)
        {

            foreach (DataTable table in data.Tables)
            {
                using (SQLiteTransaction dbTrans = conn.BeginTransaction())
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        List<string> l_Col = new List<string>();
                        foreach (DataColumn dc in table.Columns)
                            l_Col.Add(dc.ColumnName);

                        cmd.CommandText = string.Format(
                            "INSERT INTO {0}({1}) VALUES (@{2});", table.TableName,
                            string.Join(", ", l_Col),
                            string.Join(", @", l_Col));

                        Console.WriteLine("Insert Statement:\n" +
                            cmd.CommandText);

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

                    dbTrans.Commit();
                }
            }
        }

        //create the emty db file
        public static bool create_data(string filename)
        {

            if (filename != null)
            {
                SQLiteConnection.CreateFile(filename);

                return true;
            }
            else
            {
                return false;

            }

        }

        //
        /// <summary>
        ///  copy dataset to database
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Data"></param>
        /// <returns>bool</returns>
        public static bool CopydatasetToDatabase(string Connection, DataSet Data)
        {
            bool b_return = true;
            SQLiteConnection Conn = new SQLiteConnection(
                    "Data Source=" + Connection);
            try
            {
                Conn.Open();

                foreach (DataTable dt in Data.Tables)
                {
                    string m_cmd = string.Format("DROP TABLE IF EXISTS {0};", dt.TableName);
                    SQLiteCommand Cmd = new SQLiteCommand(m_cmd, Conn);
                    Cmd.ExecuteNonQuery();

                    string m_Cmd1 = string.Format("CREATE TABLE {0} (", dt.TableName);
                    // get the collum name and type 
                    List<string> ListOfColum = new List<string>();
                    List<string> ListOfPK = new List<string>();
                    DataColumn[] col = dt.PrimaryKey;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        string colum;
                        colum = dc.ColumnName + " ";
                        colum += ConverType(dc.DataType.FullName);
                        ListOfColum.Add(colum);
                    }
                    m_Cmd1 += string.Join(", ", ListOfColum);
                    if (col.Length > 0)
                    {
                        m_Cmd1 += ", ";
                        m_Cmd1 += "PRIMARY KEY (";
                        foreach (DataColumn c in col)
                        {
                            string PK;
                            PK = string.Join(", ", c.ColumnName);
                            ListOfPK.Add(PK);
                        }
                        m_Cmd1 += string.Join(", ", ListOfPK);
                        m_Cmd1 += ")";
                    }
                    m_Cmd1 += ");";

                    Cmd = new SQLiteCommand(m_Cmd1, Conn);
                    Console.WriteLine(m_Cmd1);
                    Cmd.ExecuteNonQuery();
                }

                Fill(Conn, Data);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error in CopyToDatabase:\n" +
                   exc.ToString());
                b_return = false;
            }
            finally
            {
                Conn.Close();
            }

            return b_return;
        }

        /// <summary>
        /// write data table to database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="table"></param>
        /// <returns></returns>

        public static bool CopyTableToData(string connection, DataTable table)
        {
            bool b_return = true;
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + connection);
            try
            {
                conn.Open();
                string m_cmd = string.Format("DROP TABLE IF EXISTS {0};", table.TableName);
                SQLiteCommand Cmd = new SQLiteCommand(m_cmd, conn);
                Cmd.ExecuteNonQuery();

                string m_Cmd1 = string.Format("CREATE TABLE {0} (", table.TableName);
                // get the collum name and type 
                List<string> ListOfColum = new List<string>();
                List<string> ListOfPK = new List<string>();
                DataColumn[] col = table.PrimaryKey;
                foreach (DataColumn dc in table.Columns)
                {
                    string colum;
                    colum = dc.ColumnName + " ";
                    colum += ConverType(dc.DataType.FullName);
                    ListOfColum.Add(colum);
                }
                m_Cmd1 += string.Join(", ", ListOfColum);
                //write the primary key
                if (col.Length > 0)
                {
                    m_Cmd1 += ",";
                    m_Cmd1 += "PRIMARY KEY (";
                    foreach (DataColumn c in col)
                    {
                        string PK;
                        PK = string.Join(", ", c.ColumnName);
                        ListOfPK.Add(PK);
                    }
                    m_Cmd1 += string.Join(", ", ListOfPK);
                    m_Cmd1 += ")";
                }
                m_Cmd1 += ");";
                Cmd = new SQLiteCommand(m_Cmd1, conn);
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

                    Console.WriteLine("Insert Statement:\n" +
                        cmd.CommandText);

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
            catch (Exception exc)
            {
                Console.WriteLine("Error in CopyToDatabase:\n" + exc.ToString());
                b_return = false;
            }
            conn.Close();
            return b_return;

        }

        public static DataTable GetTable(string Connection, string TableName)
        {
            DataTable table = new DataTable(TableName);
            string Command = string.Format("SELECT * FROM {0};", TableName);

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(Command, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    table.Load(reader);
                    conn.Close();
                }

            }

            catch (Exception exc)
            {
                Console.WriteLine("Exception in GetTable(): " +
                    exc.ToString());
            }
            return table;
        }
        /// <summary>
        /// Get dataSet
        /// </summary>
        /// <param name="Connection"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string Connection)
        {

            DataSet ds = new DataSet();
            List<string> get_Tables = new List<string>();
            string Command = "SELECT * FROM sqlite_master WHERE type='table'";

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(Command, conn);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    conn.Close();
                    // get list of table
                    foreach (DataRow dr in dt.Rows)
                    {
                        get_Tables.Add(dr["tbl_name"].ToString());
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("Get Table list:\n" + exc.ToString());
            }
            //read and get table in list

            foreach (string tb in get_Tables)
            {
                DataTable table = new DataTable(tb);
                string Command1 = string.Format("SELECT * FROM {0};", tb);
                try
                {
                    var connStr = new SQLiteConnectionStringBuilder()
                    {
                        DataSource = Connection
                    };

                    using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                    {
                        conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand(Command1, conn);
                        SQLiteDataReader reader1 = cmd.ExecuteReader();
                        table.Load(reader1);
                        conn.Close();
                    }

                }
                catch (Exception exc)
                {
                    Console.WriteLine("Get table:\n" + exc.ToString());
                }
                ds.Tables.Add(table);

            }

            return ds;
        }
        /// <summary>
        // Create Index for table name and colum
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Table"></param>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        public static bool CreateIndex(string Connection, string TableName, string ColumnName)
        {
            bool rt = true;
            try
            {
                string index = string.Format("idex" + ColumnName);
                string Command = string.Format("CREATE INDEX {0} ON {1}({2});", index, TableName, ColumnName);
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(Command, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine("Exception in CreateIndex(): " + exc.ToString());
                rt = false;
            }

            return rt;
        }
        /// <summary>
        /// function will do the sql statement on the database
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQLStatement"></param>
        /// <returns></returns>
        public static bool RunSelectQuery(string Connection, string SQLStatement)
        {
            bool rt = true;
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(SQLStatement, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception in RunSelectQuery: " + exc.ToString());
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

            catch (Exception exc)
            {
                Console.WriteLine("Exception in Search(): " + exc.ToString());
            }
            return result;

        }
        public static void CreateTable(string Connection, string TableName, bool AutoID,
             Dictionary<string, string> Columns)
        {
            string s_Command = "CREATE TABLE " + TableName + "(" +
                (AutoID ? "ID INTEGER PRIMARY KEY AUTOINCREMENT, " : "");

            foreach (string k in Columns.Keys)
            {
                s_Command += k + " " + Columns[k] + ", ";
            }
            s_Command = s_Command.Substring(0, s_Command.Length - 2);   // removes the last comma and space
            s_Command += ");";

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = Connection
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn);
                    cmd.CommandText = s_Command;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (IOException ioe)
            {
                Console.WriteLine("SQLite Handler IOException in CreateTable(): " +
                    ioe.ToString());
            }
            catch (Exception exc)
            {
                Console.WriteLine("SQLite Handler Exception in CreateTable(): " +
                    exc.ToString());
            }
        }



        // Insert to table
        public static bool Insert(string sqConnectionString, string tableName, Dictionary<string, string> data)
        {
            string columns = "";
            string values = "";
            bool returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
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
                    DataSource = sqConnectionString
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn);
                    cmd.CommandText = myInsertQuery;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            catch (Exception fail)
            {
                System.Console.WriteLine(fail.Message);
                returnCode = false;
            }

            return returnCode;
        }


        //update table       
        public static bool Update(string sqConnectionString, string tableName, Dictionary<string, string> data, string where)
        {
            string vals = "";
            bool returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            string myUpdateQuery = string.Format("UPDATE {0} SET {1} WHERE {2};", tableName, vals, where);

            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = sqConnectionString
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn);
                    cmd.CommandText = myUpdateQuery;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }

            catch (Exception fail)
            {
                System.Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        //delete from table
        public static bool Delete(string sqConnectionString, string tableName, string where)
        {
            bool returnCode = true;
            string myDeleteQuery = string.Format("DELETE FROM {0} WHERE {1};", tableName, where);
            try
            {
                var connStr = new SQLiteConnectionStringBuilder()
                {
                    DataSource = sqConnectionString
                };

                using (SQLiteConnection conn = new SQLiteConnection(connStr.ToString()))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(conn);
                    cmd.CommandText = myDeleteQuery;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (Exception fail)
            {
                System.Console.WriteLine(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }
    }
}
