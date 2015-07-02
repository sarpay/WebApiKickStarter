using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

using System.Linq;
using System.Data;
using System.Data.SqlClient;

using WebApiProject.Models;
using WebApiProject.Models.Tables;
using WebApiProject.Models.Views;

namespace WebApiProject.Controllers
{
    public class HomeController : ApiController
    {
        private DataModel db = new DataModel();
        protected ADONET AdoNet = new ADONET();

        //*********************************************
        // POST QUERIES USING STORED PROCEDURES
        //*********************************************

        // [URI: api/purchases], [VIEW: purchases.html]
        [Route("api/purchases")]
        [HttpPost]
        public object[] GetPurchases(
            int good_id,
            byte? gender_ix)
        {
            //System.Threading.Thread.Sleep(2000);

            List<object> resultsList = new List<object>();
            List<object> dataList = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            try
            {
                // query the database
                AdoNet.SqlConnect();

                //*** Get [Users] table
                AdoNet.SqlNewCommand("dbo.getPurchases", "sp");

                // INs
                AdoNet.SqlNewParam("Input", "@GoodID", good_id, SqlDbType.Int, 0);
                AdoNet.SqlNewParam("Input", "@GenderIX", gender_ix, SqlDbType.TinyInt, 0);

                // Create a DataList
                AdoNet.SqlNewAdapter(AdoNet.SqlCmd);
                AdoNet.SqlFillDataTable();
                dataList = Helpers.DataTableToList(AdoNet.SqlDataTable);

                // Add DataList to Dictionary
                dict.Add("Result", "OK");
                dict.Add("Data", dataList);
            }
            catch (SqlException x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "DATABASE: " + x.ToString());
            }
            catch (Exception x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "APP: " + x.ToString());
            }

            resultsList.Add(dict);
            return resultsList.ToArray();
        }

        // [URI: api/new-shopper?action=insert], [VIEW: new-shopper.html]
        [Route("api/new-shopper")]
        [HttpPost]
        public object[] NewShopper(
            string email,
            string pwd,
            string name,
            byte? gender_ix,
            bool opt_in,
            string action = null)
        {
            //System.Threading.Thread.Sleep(2000);

            List<object> list = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            int acct_id = 0;

            try
            {
                SqlParameter[] params_sp1 = {
                    new SqlParameter() {
                        ParameterName = "NewID",
                        SqlDbType = SqlDbType.Int,
                        Value = 0,
                        Direction = ParameterDirection.Output
                    },
                    new SqlParameter() {
                        ParameterName = "Email",
                        SqlDbType = SqlDbType.VarChar,
                        Value = email,
                        Direction = ParameterDirection.Input
                    },
                    new SqlParameter() {
                        ParameterName = "Password",
                        SqlDbType = SqlDbType.NVarChar,
                        Value = pwd,
                        Direction = ParameterDirection.Input
                    }
                };

                int exec_sp1 = db.Database.ExecuteSqlCommand(
                    "dbo.newAccount @Email, @Password, @NewID out", params_sp1);

                //*** obtain output paramater's (@NewID) value
                acct_id = (int)params_sp1[0].Value; //*** [0] is the index number of the output param


                SqlParameter[] params_sp2 = {
                    new SqlParameter() {
                        ParameterName = "AccountID",
                        SqlDbType = SqlDbType.Int,
                        Value = acct_id,
                        Direction = ParameterDirection.Input
                    },
                    new SqlParameter() {
                        ParameterName = "Name",
                        SqlDbType = SqlDbType.NVarChar,
                        //Size = 100,
                        Value = name,
                        Direction = ParameterDirection.Input
                    },
                    new SqlParameter() {
                        ParameterName = "GenderIX",
                        SqlDbType = SqlDbType.TinyInt,
                        Value = gender_ix,
                        Direction = ParameterDirection.Input
                    },
                    new SqlParameter() {
                        ParameterName = "OptIn",
                        SqlDbType = SqlDbType.Bit,
                        Value = opt_in,
                        Direction = ParameterDirection.Input
                    }
                };

                int exec_sp2 = db.Database.ExecuteSqlCommand(
                        "dbo.newShopper @AccountID, @Name, @GenderIX, @OptIn", params_sp2);

                dict.Add("Result", "OK");
            }
            catch (SqlException x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "DATABASE: " + x.ToString());
            }
            catch (Exception x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "APP: " + x.ToString());
            }

            dict.Add("NewID", acct_id);
            list.Add(dict);

            return list.ToArray();

        }


        //*********************************************
        // GET QUERIES USING STORED PROCEDURES
        //*********************************************

        // [URI: api/shoppers], [VIEW: shoppers.html]
        [Route("api/shoppers")]
        public IEnumerable<getShoppers> GetShoppersFromView()
        {
            //try
            //{
            SqlParameter[] sp = {
                new SqlParameter() {
                    ParameterName = "GenderIX",
                    SqlDbType = SqlDbType.TinyInt,
                    Value = DBNull.Value,
                    Direction = ParameterDirection.Input
                },
                new SqlParameter() {
                    ParameterName = "OptIn",
                    SqlDbType = SqlDbType.Bit,
                    Value = DBNull.Value,
                    Direction = ParameterDirection.Input
                }
            };

            //var results = db.Database.SqlQuery<getShoppers>(
            //    "dbo.getShoppers @GenderIX, @OptIn", sp)
            //    .ToList();

            // ---------- OR ----------

            // This REQUIRES getShoppers DbSet DEFINED IN DataModel.cs
            var results = db.getShoppers.SqlQuery(
                "dbo.getShoppers @GenderIX, @OptIn", sp)
                .ToList();

            return results;

            //}
            //catch (Exception e)
            //{
            //    IEnumerable<getShoppers> sequenceOfFoos = new getShoppers[] { new getShoppers() { ID = 0 }, new getShoppers() { Email = e.Message } };
            //    return sequenceOfFoos;
            //}
            
        }

        // [URI: api/shopper/1], [VIEW: shopper.html]
        [Route("api/shopper/{acct_id}")]
        public IEnumerable<getShoppers> GetShopperFromView(int acct_id)
        {

            SqlParameter[] sp = {
                new SqlParameter() {
                    ParameterName = "AccountID",
                    SqlDbType = SqlDbType.Int,
                    Value = acct_id,
                    Direction = ParameterDirection.Input
                }
            };

            //var results = db.Database.SqlQuery<getShoppers>(
            //    "dbo.getShopper @AccountID", sp)
            //    .ToList();

            // ---------- OR ----------

            var results = db.getShoppers.SqlQuery(
                "dbo.getShopper @AccountID", sp)
                .ToList();

            return results;
        }


        //*********************************************
        // GET QUERIES USING RAW SQL
        //*********************************************


        //*********************************************
        // GET QUERIES ON TABLES
        //*********************************************

        // [URI: api/genders], [VIEW: genders.html]
        [Route("api/genders")]
        public IQueryable<Genders> GetGendersFromTable()
        {
            return db.Genders;
        }

        // [URI: api/gender/1], [VIEW: gender.html]
        [Route("api/gender/{ix}")]
        [ResponseType(typeof(Genders))]
        public IHttpActionResult GetGendersFromTable(byte? ix)
        {
            Genders genders = db.Genders.Find(ix);
            //if (genders == null)
            //{
            //    return  NotFound(); //returns a 404 error
            //}
            return Ok(genders);
        }













        // POST: api/new_shopper
        //[Route("api/new_shopper")]
        //public void Post([FromBody]string value)
        //{
        //}


        // PUT: api/Default/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}


        // DELETE: api/Default/5
        //public void Delete(int id)
        //{
        //}
    }
}
