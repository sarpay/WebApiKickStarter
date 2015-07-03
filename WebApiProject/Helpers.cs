using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace WebApiProject
{
    public class Helpers
    {

        protected ADONET AdoNet = new ADONET();


        public static List<object> DataTableToList(DataTable dt)
        {
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

            return rowList;
        }


        public static bool ConvertToBoolean(object value)
        {
            bool functionReturnValue = ((string)value == "1" | (short)value == 1);
            return functionReturnValue;
        }


        public static object[] DataTableToJaggedArray(DataTable dt) // jagged aka array-of-arrays
        {
            object[][] array = new object[1][];
            Dictionary<string, object> colList;
            List<object> rowList;
            List<object> tableList = new List<object>();
            int runCount = -1;

            foreach (DataRow dr in dt.Rows)
            {
                colList = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    colList.Add(dc.ColumnName, (string.Empty == dr[dc].ToString()) ? "" : dr[dc].ToString());
                }

                rowList = new List<object>();
                rowList.Add(colList);
                tableList.Add(rowList);

                runCount += 1;
            }

            //Array.Resize(ref array, runCount + 1);


            //orderedList = tableList.OrderBy(o => o.ID).ToList();
            //Array.Sort<int>(0, (a, b) => array[a].CompareTo(array[b]));
            array[0] = tableList.ToArray();
            //for (int i = 0; i < array.GetUpperBound(1); i++)
            //{
            //    array[i] = tableList.ToArray();
            //}
            

            return array[0];
        }
    }
}