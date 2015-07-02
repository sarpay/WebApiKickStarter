using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace WebApiProject
{
    public class Helpers
    {
        private static List<object> DataTableToList(DataTable dt)
        {

            //List<object> studentList = new List<object>();
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    Student student = new Student();
            //    student.StudentId = Convert.ToInt32(dt.Rows[i]["StudentId"]);
            //    student.StudentName = dt.Rows[i]["StudentName"].ToString();
            //    student.Address = dt.Rows[i]["Address"].ToString();
            //    student.MobileNo = dt.Rows[i]["MobileNo"].ToString();
            //    studentList.Add(student);
            //}

            List<object> rowList = new List<object>();

            //foreach (DataRow dr in dt.Rows)
            //{
            //    Dictionary<string, object> colList = new Dictionary<string, object>();
            //    foreach (DataColumn dc in dt.Columns)
            //    {
            //        colList.Add(dc.ColumnName, (string.Empty == dr(dc).ToString()) ? "" : dr(dc).ToString());
            //    }
            //    rowList.Add(colList);
            //}

            return rowList;

        }

        private static List<T> DataTableToList_GENERIC<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        private static List<DataRow> DataTableToList_Legacy(DataTable dt)
        {
            List<DataRow> list = dt.AsEnumerable().ToList();
            return list;


            //List<object> rowList = new List<object>();

            //foreach (DataRow dr in dt.Rows)
            //{
            //    Dictionary<string, object> colList = new Dictionary<string, object>();
            //    foreach (DataColumn dc in dt.Columns)
            //    {
            //        colList.Add(dc.ColumnName, (string.Empty == dr(dc).ToString()) ? "" : dr(dc).ToString());
            //    }
            //    rowList.Add(colList);
            //}

            //return rowList;
        }
    }
}