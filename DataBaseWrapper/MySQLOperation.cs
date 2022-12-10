using MySqlConnector;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MATS2_QueryBuilder.DataBaseWrapper
{
    public class MySQLOperation
    {
        public static string ConnectionString = string.Empty;
        public bool DBCreateFlag=false;

        public MySQLOperation()
        {
            ConnectionString = GetDBConnectionString();
            if (DBCreateFlag)
            {
                bool ReturnVal = CreateDBDynamically("test_1");
                if (ReturnVal) 
                    Console.WriteLine("Successfully created DB");
                else
                    Console.WriteLine("Could not create DB");
            }
        }       

        /// <summary>
        /// Returns the connection string object
        /// use this to read from a file and populate the connection string
        /// </summary>
        /// <returns>DBConnection string</returns>
        public string GetDBConnectionString()
        {
            string ConnString = string.Empty;
            try
            {
                JObject? ConnJSON = Utility.ReadMYSQLDBConfigFile();
                string? DBServer = string.Empty;
                string? UserId = string.Empty;
                string? Password = string.Empty;
                string? DBName = string.Empty;
                //bool DoYouNeedToCreateNewDB = false;
                if (ConnJSON != null)
                {
                    DBCreateFlag = (bool)(ConnJSON["createNedDB"]);
                    DBServer = Convert.ToString(ConnJSON["dbServer"]);
                    UserId = Convert.ToString(ConnJSON["userName"]);
                    Password = Convert.ToString(ConnJSON["password"]);
                    DBName = Convert.ToString(ConnJSON["dbName"]);
                    
                }
                else
                {
                    Console.WriteLine("Please check Database connection file");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                if (!DBCreateFlag)
                    ConnString = @"server=" + DBServer + ";database=" + DBName + ";userid=" + UserId + ";password=" + Password + ";" + "SslMode=none" + ";AllowPublicKeyRetrieval=True";
                else
                    ConnString = @"server=" + DBServer + ";userid=" + UserId + ";password=" + Password + ";" + "SslMode=none" + ";AllowPublicKeyRetrieval=True";
            }
            catch (Exception ex)
            {
                if (GlobalVar.IsDatabaseDebugEnabled)
                    Console.WriteLine("GetDBConnectionString EXCP : {0}", ex.Message);
            }
            return ConnString;
        }// end of GetDBConnectionString


        public bool CreateDBDynamically(string DBNameToCreate)
        {
            bool ReturnSuccess = false;
            string createDBCmd = "CREATE DATABASE " + DBNameToCreate;
            try
            {
                using (MySqlConnection DBConnObj = new MySqlConnection(ConnectionString))
                {
                    DBConnObj.Open();
                    using (MySqlCommand dbcmd = new MySqlCommand(createDBCmd, DBConnObj))
                    {
                        dbcmd.CommandTimeout = 60;
                        //dbcmd.CommandText = createDBCmd;
                        dbcmd.ExecuteNonQuery();
                        ReturnSuccess = true;
                    }

                }
                Console.WriteLine("Database Sample_DB created successfully");
                
            }
            catch (Exception ex)
            {
                if (GlobalVar.IsDatabaseDebugEnabled)
                    Console.WriteLine("CreateDBDynamically EXCP : {0}", ex.Message);
            }
            return ReturnSuccess;

        }

        /// <summary>
        /// TO Get Array of records from db
        /// </summary>
        /// <param name="QueryString">Fully constructed Query</param>
        /// <returns>Json Array of Records</returns>
        public JArray GetAll(string QueryString)
        {
            string sql = QueryString;
            JArray jsondata = new JArray();
            try
            {
                using (MySqlConnection DBConnObj = new MySqlConnection(ConnectionString))
                {
                    DBConnObj.Open();
                    using (MySqlCommand getComm = new MySqlCommand(sql, DBConnObj))
                    {
                        //Stopwatch st = new();
                        //st.Start();

                        using (MySqlDataReader dr = getComm.ExecuteReader())
                        {

                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    JObject jsonObject = new JObject();
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        // jsonObject[dr.GetName(i)] = dr[i].ToString();
                                        Type tp = dr[i].GetType();
                                        if (tp.ToString().Contains("Int"))
                                            jsonObject[dr.GetName(i)] = Convert.ToInt32(dr[i]);
                                        else if (tp.ToString().Contains("Bool"))
                                            jsonObject[dr.GetName(i)] = Convert.ToBoolean(dr[i]);
                                        else if (tp.ToString().Contains("DateTime"))
                                            jsonObject[dr.GetName(i)] = Convert.ToDateTime(dr[i].ToString());
                                        else
                                            jsonObject[dr.GetName(i)] = dr[i].ToString();
                                    }
                                    jsondata.Add(jsonObject);
                                }
                            }
                            //st.Stop();
                            //Console.WriteLine(sql);
                            //Console.WriteLine(@"Profile db Execute Array {0}", st.ElapsedMilliseconds);
                        } // reader closed and disposed up here
                    } // command disposed here 
                }  ////connection closed and disposed here
            }
            catch (Exception e)
            {
                Console.WriteLine(QueryString);
                Console.WriteLine("DATABASE EXECP GetAll : {0}", e.Message);
            }
            return jsondata;
        }//end of get all

        //Added for excel file upload/download feature
        public DataTable GetAllDataTable(string QueryString)
        {
            string sql = QueryString;
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection DBConnObj = new MySqlConnection(ConnectionString))
                {
                    DBConnObj.Open();
                    using (MySqlCommand getComm = new MySqlCommand(sql, DBConnObj))
                    {
                        //Stopwatch st = new();
                        //st.Start();

                        dt.Load(getComm.ExecuteReader());
                        //st.Stop();
                        //Console.WriteLine(sql);
                        //Console.WriteLine(@"Profile db Execute datatable {0}", st.ElapsedMilliseconds);
                        return dt;
                    } // command disposed here 
                }  ////connection closed and disposed here
            }
            catch (Exception e)
            {
                Console.WriteLine(QueryString);
                Console.WriteLine("DATABASE EXECP GetAll DATATABLE: {0}", e.Message);
            }
            return dt;
        }//end of get all

        /// <summary>
        /// Gets single record of requested coloumns based on constructed sql
        /// </summary>
        /// <param name="QueryString">Fully constructed SQL string</param>
        /// <returns>JSON Object</returns>
        public JObject GetOne(string QueryString)
        {
            string sql = QueryString;
            JObject jsonObject = new JObject();
            try
            {
                using (MySqlConnection DBConnObj = new MySqlConnection(ConnectionString))
                {
                    DBConnObj.Open();
                    using (MySqlCommand getComm = new MySqlCommand(sql, DBConnObj))
                    {
                        using (MySqlDataReader dr = getComm.ExecuteReader())
                        {
                            if (dr != null)
                            {
                                while (dr.Read())
                                {
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        Type tp = dr[i].GetType();
                                        if (tp.ToString().Contains("Int"))
                                            jsonObject[dr.GetName(i)] = Convert.ToInt32(dr[i]);
                                        //else if (tp.ToString().Contains("Int64"))
                                        //    jsonObject[dr.GetName(i)] = Convert.ToInt64(dr[i]);
                                        else if (tp.ToString().Contains("Bool"))
                                            jsonObject[dr.GetName(i)] = Convert.ToBoolean(dr[i]);
                                        else if (tp.ToString().Contains("DateTime"))
                                            jsonObject[dr.GetName(i)] = Convert.ToDateTime(dr[i].ToString());
                                        else
                                            jsonObject[dr.GetName(i)] = dr[i].ToString();
                                    }
                                }
                            }
                        } // reader closed and disposed up here
                    } // command disposed here 
                }  ////connection closed and disposed here

            }
            catch (Exception e)
            {
                Console.WriteLine("DATABASE EXECP GetOne : {0}", e.Message);
            }
            return jsonObject;
        }//end of get One

        
        /// <summary>
        /// Inserting/Updating/Deleting record in database
        /// </summary>
        /// <param name="QueryString">Fully constructed query</param>
        /// <returns>Success or failure</returns>
        public string CUDOpp(string QueryString)
        {
            string sql = QueryString;
            try
            {
                using (MySqlConnection DBConnObj = new MySqlConnection(ConnectionString))
                {
                    DBConnObj.Open();
                    using (MySqlCommand dbcmd = new MySqlCommand(sql, DBConnObj))
                    {
                        dbcmd.CommandTimeout = 60;
                        dbcmd.CommandText = sql;
                        dbcmd.ExecuteNonQuery();
                    } // command disposed here   
                }  ////connection closed and disposed here
                return "Success";
            }
            catch (Exception ex)
            {
                // if (GlobalVar.IsDebugModeEnabled)
                Console.WriteLine("AddData ERROR: {0} ", ex.Message);
                return "Failure";
            }
        }//public string CUDOpp(string QueryString)
    }//End of Class MySQLOperations

    /// <summary>
    /// This class will be used to construct the Insert Statement
    /// </summary>
    class Insert
    {
        Hashtable args = new Hashtable();
        string table;

        /// <summary>
        /// Constructs Insert object
        /// </summary>
        /// <param name="table">table name to insert to</param>
        public Insert(string table)
        {
            this.table = table;
        }

        /// <summary>
        /// Adds item to Insert object
        /// </summary>
        /// <param name="name">item name</param>
        /// <param name="val">item value</param>
        public void Add(string name, object val)
        {
            args.Add(name, val);
        }

        /// <summary>
        /// Removes item from Insert object
        /// </summary>
        /// <param name="name">item name</param>
        public void Remove(string name)
        {
            try
            {
                args.Remove(name);
            }
            catch
            {
                throw (new Exception("No such item"));
            }
        }

        /// <summary>
        /// Test representatnion of the Insert object (SQL query)
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            StringBuilder s1 = new StringBuilder();
            StringBuilder s2 = new StringBuilder();

            IDictionaryEnumerator enumInterface = args.GetEnumerator();
            bool first = true;
            while (enumInterface.MoveNext())
            {
                if (first) first = false;
                else
                {
                    s1.Append(", ");
                    s2.Append(", ");
                }
                s1.Append(enumInterface.Key.ToString());
                s2.Append(enumInterface.Value.ToString());
            }

            return "INSERT INTO `" + table + "` (" + s1 + ") VALUES (" + s2 + ");";
        }

        /// <summary>
        /// Gets or sets item into Insert object
        /// </summary>
        object this[string key]
        {
            get
            {
                Debug.Assert(args.Contains(key), "Key not found");
                return args[key];
            }
            set { args[key] = value; }
        }
    }

    /// <summary>
    /// This class will construct the update staement 
    /// We need to create where clause???// TBD: move the where clause also in this query
    /// </summary>
    class Update
    {
        Hashtable args = new Hashtable();
        string table;
        string WhereClause;

        /// <summary>
        /// Constructs Insert object
        /// </summary>
        /// <param name="table">table name to insert to</param>
        public Update(string table, string WhereClause)
        {
            this.table = table;
            this.WhereClause = WhereClause;
        }

        /// <summary>
        /// Adds item to Insert object
        /// </summary>
        /// <param name="name">item name</param>
        /// <param name="val">item value</param>
        public void Add(string name, object val)
        {
            args.Add(name, val);
            //Since it is Hashtable it is not letting me add one col only????
        }

        /// <summary>
        /// Removes item from Insert object
        /// </summary>
        /// <param name="name">item name</param>
        public void Remove(string name)
        {
            try
            {
                args.Remove(name);
            }
            catch
            {
                throw (new Exception("No such item"));
            }
        }

        /// <summary>
        /// Test representatnion of the Insert object (SQL query)
        /// </summary>
        /// <returns>System.String</returns>
        public override string ToString()
        {
            StringBuilder s1 = new StringBuilder();
            //StringBuilder s2 = new StringBuilder();
            string UpdateString = "UPDATE `" + table + "` SET ";
            IDictionaryEnumerator enumInterface = args.GetEnumerator();
            bool first = true;
            while (enumInterface.MoveNext())
            {
                if (first) first = false;
                else
                {
                    s1.Append(", ");
                }
                s1.Append(enumInterface.Key.ToString());
            }
            UpdateString = UpdateString + s1 + " WHERE " + WhereClause;
            return UpdateString;
        }

        /// <summary>
        /// Gets or sets item into Insert object
        /// </summary>
        object this[string key]
        {
            get
            {
                Debug.Assert(args.Contains(key), "Key not found");
                return args[key];
            }
            set { args[key] = value; }
        }
    }//ENd of Update Class
}
