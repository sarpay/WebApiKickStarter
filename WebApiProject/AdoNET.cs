using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebApiProject
{
    
    public class ADONET
    {

        public SqlConnection SqlConn;
        public SqlCommand SqlCmd;
        public SqlDataAdapter SqlAdapter;
        public DataSet SqlDataSet;
        public DataTable SqlDataTable;
        public SqlDataReader SqlReader;
        public SqlParameter SqlParam;

        public string SqlConnStr()
        {
            string str = ConfigurationManager.ConnectionStrings["DataModel"].ConnectionString;
            //string str = ConfigurationManager.ConnectionStrings["DataModel_EX"].ConnectionString;
            return str;
        }

        public void SqlConnect()
        {
            SqlConn = new SqlConnection(SqlConnStr());
            SqlConn.Open();
        }

        public void SqlNewCommand(string cmdText, string cmdType)
        {
            SqlCmd = new SqlCommand();
            SqlCmd.Connection = SqlConn;
            SqlCmd.CommandText = cmdText;
            SqlCmd.CommandTimeout = 60;//in seconds
            switch (cmdType)
            {
                case "sp":
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    break; // TODO: might not be correct. Was : Exit Select
                case "text":
                    SqlCmd.CommandType = CommandType.Text;
                    break; // TODO: might not be correct. Was : Exit Select
                default:
                    SqlCmd.CommandType = CommandType.StoredProcedure;
                    break; // TODO: might not be correct. Was : Exit Select
            }
        }

        public void SqlNewAdapter(SqlCommand cmd)
        {
            SqlAdapter = new SqlDataAdapter(cmd);
        }

        public void SqlNewDataSet()
        {
            SqlDataSet = new DataSet();
        }

        public void SqlFillDataSet(string dataTableName)
        {
            SqlAdapter.Fill(SqlDataSet, dataTableName);
        }

        public void SqlFillDataTable()
        {
            SqlDataTable = new DataTable();
            SqlAdapter.Fill(SqlDataTable);
        }

        public void SqlNewParam(
            string direction, 
            string paramName, 
            object fieldValue, 
            SqlDbType dbType, 
            int fieldLength)
        {
            SqlParam = new SqlParameter();

            if (fieldLength != 0)
            {
                SqlParam = SqlCmd.Parameters.Add(paramName, dbType, fieldLength);
            }
            else
            {
                SqlParam = SqlCmd.Parameters.Add(paramName, dbType);
            }

            switch (direction)
            {
                case "Input":
                    SqlParam.Direction = ParameterDirection.Input;
                    //Allows json to send [null] values.
                    if ((fieldValue != null))
                    {
                        //Treats empty strings as [null].
                        if (Convert.ToString(fieldValue).Length > 0)
                        {
                            switch (dbType)
                            {
                                case SqlDbType.Int:
                                    SqlParam.Value = Convert.ToInt32(fieldValue);
                                    break;
                                case SqlDbType.SmallInt:
                                case SqlDbType.TinyInt:
                                    SqlParam.Value = Convert.ToInt16(fieldValue);
                                    break;
                                case SqlDbType.Bit:
                                    SqlParam.Value = Convert.ToByte(Helpers.ConvertToBoolean(fieldValue));
                                    break;
                                default:
                                    SqlParam.Value = Convert.ToString(fieldValue);
                                    break;
                            }
                        }
                        else
                        {
                            SqlParam.Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        SqlParam.Value = DBNull.Value;
                    }
                    break;

                case "Output":
                case "InputOutput":
                    SqlParam.Direction = ParameterDirection.Output;
                    break;
                case "ReturnValue":
                    SqlParam.Direction = ParameterDirection.ReturnValue;
                    break;
            }

        }

        public object SqlOutputParamValue(string paramName)
        {
            return SqlCmd.Parameters[paramName].Value;
        }

        public void SqlExecuteCommand()
        {
            SqlCmd.ExecuteNonQuery();
        }

        public void SqlExecuteReader()
        {
            SqlReader = SqlCmd.ExecuteReader();
        }

        public bool SqlReaderHasRows()
        {
            bool functionReturnValue = false;

            functionReturnValue = false;
            if (SqlReader.HasRows)
            {
                functionReturnValue = true;
            }
            return functionReturnValue;
        }

        public bool SqlReaderRead()
        {
            return SqlReader.Read();
        }

        public string SqlReaderItem(string columnName)
        {
            return Convert.ToString(SqlReader[columnName]);
        }

        public void SqlCloseReader()
        {
            SqlReader.Close();
        }

        public void SqlDisconnect()
        {
            if (SqlConn != null)
            {
                if ((SqlConn.State == ConnectionState.Open))
                {
                    if (SqlCmd.Parameters != null)
                    {
                        SqlCmd.Parameters.Clear();
                    }
                    if (SqlCmd != null)
                    {
                        SqlCmd.Dispose();
                    }
                    if (SqlReader != null)
                    {
                        if (!(SqlReader.IsClosed))
                        {
                            SqlReader.Close();
                        }
                    }
                    SqlConn.Close();
                }
                SqlConn.Dispose();
            }
        }

    }
}