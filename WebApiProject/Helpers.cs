using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace WebApiProject
{
    public class Helpers
    {
        public static List<object> DataTableToList(DataTable dt)
        {
            List<object> rowList = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> colList = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    colList.Add(dc.ColumnName, (string.Empty == dr[dc].ToString()) ? "" : dr[dc].ToString());
                }
                rowList.Add(colList);
            }
            return rowList;
        }


        public static bool ConvertToBoolean(object value)
        {
            bool functionReturnValue = ((string)value == "1" | (short)value == 1);
            return functionReturnValue;
        }
    }
}