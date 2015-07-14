using System;
using System.Collections.Generic;
using System.Data;
using WebApiProject.Models.Views;

namespace WebApiProject
{
    public class Helpers
    {

        protected ADONET AdoNet = new ADONET();

        public static List<object> DataTableToList(DataTable dt)
        {
            List<object> rowList = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                var colDict = new Dictionary<string, string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    colDict.Add(dc.ColumnName, (string.Empty == dr[dc].ToString()) ? "" : dr[dc].ToString());
                }
                rowList.Add(colDict);
            }
            return rowList;
        }


        public static object[] DataTableToArray(DataTable dt)
        {
            object[][] array = new object[1][]; //*** jagged aka array-of-arrays
            Dictionary<string, object> colList;
            List<object> rowList = new List<object>();

            foreach (DataRow dr in dt.Rows)
            {
                colList = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    colList.Add(dc.ColumnName, (string.Empty == dr[dc].ToString()) ? "" : dr[dc].ToString());
                }
                rowList.Add(colList);
            }
            array[0] = rowList.ToArray();

            return array[0];
        }


        public static bool ConvertToBoolean(object value)
        {
            bool functionReturnValue = ((string)value == "1" | (short)value == 1);
            return functionReturnValue;
        }


        public static object ConvertToDbNullWhenNull(object obj)
        {
            return (obj == null ? DBNull.Value : obj);
        }


        public static bool TryConvertTo<T>(string input)
        {
            Object result = null;
            try
            {
                result = Convert.ChangeType(input, typeof(T));
            }
            catch
            {
                return false;
            }
            return true;
        }

    }
}