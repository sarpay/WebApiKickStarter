using System;
using System.Configuration;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

using System.Linq;
using System.Data;
using System.Data.SqlClient;

using WebApiProject.Models;
using WebApiProject.Models.Tables;
using WebApiProject.Models.Views;
using System.ComponentModel;

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
        [Route("api/sign-in")]
        [HttpPost]
        public object[] SignIn(
            string username,
            string password)
        {

            //System.Threading.Thread.Sleep(2000);

            List<object> resultsList = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            //** make modifications on posted data (ready user input for db)
            password += ConfigurationManager.AppSettings["mySalt"];

            try
            {
                //** query the database
                AdoNet.SqlConnect();

                //** specify the stored procedure
                AdoNet.SqlNewCommand("dbo.getPassword", "sp");
                //** INs
                AdoNet.SqlNewParam("Input", "@Username", username, SqlDbType.VarChar, 255);
                //** OUTs
                AdoNet.SqlNewParam("Output", "@UserID", null, SqlDbType.Int, 0);
                AdoNet.SqlNewParam("Output", "@Password", null, SqlDbType.Char, 60);
                //** Execute SP
                AdoNet.SqlExecuteCommand();
                //** Obtain output param's value

                int userId = 0;
                if (Helpers.TryConvertTo<int>(AdoNet.SqlOutputParamValue("@UserID").ToString())) {
                    userId = Convert.ToInt32(AdoNet.SqlOutputParamValue("@UserID").ToString());
                }

                if (userId > 0) {
                    string hashedPwdFromDB = AdoNet.SqlOutputParamValue("@Password").ToString();
                    bool passwordsMatch = BCrypt.Net.BCrypt.Verify(password, hashedPwdFromDB);
                    if (passwordsMatch) {
                        //** specify the stored procedure
                        AdoNet.SqlNewCommand("dbo.newTicket", "sp");
                        //** INs
                        AdoNet.SqlNewParam("Input", "@UserID", userId, SqlDbType.Int, 0);
                        //** OUTs
                        AdoNet.SqlNewParam("Output", "@Ticket", null, SqlDbType.Char, 36);
                        //** Execute SP
                        AdoNet.SqlExecuteCommand();
                        //** Obtain output param's value
                        string ticket = AdoNet.SqlOutputParamValue("@Ticket").ToString();
                        // Populate the Dictionary
                        dict.Add("Result", "OK");
                        dict.Add("Ticket", ticket);
                    } else {
                        dict.Add("Result", "BLOCKED");
                        dict.Add("Msg", "Wrong Password!");
                    }
                } else {
                    dict.Add("Result", "BLOCKED");
                    dict.Add("Msg", "No Such User!");
                }
            }
            catch (SqlException x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "SQL: " + x.ToString());
            }
            catch (Exception x)
            {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "APP: " + x.ToString());
            }
            finally
            {
                AdoNet.SqlDisconnect();
            }

            resultsList.Add(dict);
            return resultsList.ToArray();
        }


        // [URI: api/purchases], [VIEW: purchases.html]
        [Route("api/purchases")]
        [HttpPost]
        public object[] GetPurchases(
            int? good_id,
            byte? gender_ix) {

            //System.Threading.Thread.Sleep(2000);

            List<object> resultsList = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();

            try {
                //*** query the database
                AdoNet.SqlConnect();

                //*** Get [Users] table
                AdoNet.SqlNewCommand("dbo.getPurchases", "sp");

                //*** INs
                AdoNet.SqlNewParam("Input", "@GoodID", good_id, SqlDbType.Int, 0);
                AdoNet.SqlNewParam("Input", "@GenderIX", gender_ix, SqlDbType.TinyInt, 0);

                //*** Set Adapter
                AdoNet.SqlNewAdapter(AdoNet.SqlCmd);

                //*** Fill DataTable from Adapter
                AdoNet.SqlFillDataTable();

                //*** Fill the List object with DataTable results
                List<object> dataList = new List<object>();
                dataList = Helpers.DataTableToList(AdoNet.SqlDataTable);

                //*** Fill the Array object with DataTable results
                //object[] dataArray = Helpers.DataTableToArray(AdoNet.SqlDataTable);
                //int arrayCount = dataArray.Count();

                //*** Custom Sorting
                //dataList.Sort();

                //*** Get items count
                int listCount = dataList.Count();

                //*** Modify List Items
                foreach (Dictionary<string, string> colDict in dataList) {
                    
                    //*** Iterating thru rows of the List object..

                    List<string> keysOfMoney = new List<string>();
                    List<string> keysOfEmail = new List<string>();
                    List<string> keysOfDateTime = new List<string>();

                    foreach (string key in colDict.Keys) {
                        if (Helpers.TryConvertTo<decimal>(colDict[key]) && colDict[key].Contains(".")) {
                            keysOfMoney.Add(key);
                        }
                        if (Helpers.TryConvertTo<DateTime>(colDict[key])) {
                            keysOfDateTime.Add(key);
                        }
                        if (colDict[key].Contains("@")) {
                            keysOfEmail.Add(key);
                        }
                    }

                    //*** modify ALL Email values with custom html
                    foreach (string key in keysOfEmail) {
                        colDict[key] = "<a href=\"mailto:" + colDict[key] + "\">" + colDict[key] + "</a>";
                    }
                    //*** --- or --- (modify Email values of a specific column)
                    //colDict["ShopperEmail"] = "<a href=\"mailto:" + colDict["ShopperEmail"] + "\">" + colDict["ShopperEmail"] + "</a>";

                    //*** modify ALL SmallMoney values
                    foreach (string key in keysOfMoney) {
                        colDict[key] = "$" + String.Format("{0:F2}", Convert.ToDecimal(colDict[key]));
                    }
                    //*** --- or --- (modify SmallMoney values of a specfic column)
                    //colDict["TotalPurchase"] = "$" + String.Format("{0:F2}", Convert.ToDecimal(colDict["TotalPurchase"]));

                    //*** modify ALL DateTime values
                    foreach (string key in keysOfDateTime) {
                        colDict[key] = String.Format("{0:MMMM dd - yyyy @ HH:mm}", Convert.ToDateTime(colDict["RegDate"]));
                    }
                    //*** --- or --- (modify DateTime values of a specfic column)
                    //colDict["RegDate"] = String.Format("{0:MMMM dd - yyyy @ HH:mm}", Convert.ToDateTime(colDict["RegDate"]));

                    //*** modify Gender column with custom html
                    if (colDict["ShopperGender"] == "Male") {
                        colDict["ShopperGender"] = "<img src=\"images/male.png\" style=\"width: 20px; height: 20px\" />";
                    } else {
                        colDict["ShopperGender"] = "<img src=\"images/female.png\" style=\"width: 20px; height: 20px\" />";
                    }

                }

                // Populate the Dictionary
                dict.Add("Result", "OK");
                dict.Add("Data", dataList);
                //dict.Add("Array", dataArray);
                //dict.Add("ArrayCount", arrayCount);
                dict.Add("Count", listCount);
            }
            catch (SqlException x) {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "SQL: " + x.ToString());
            }
            catch (Exception x) {
                dict.Add("Result", "ERROR");
                dict.Add("ErrMsg", "APP: " + x.ToString());
            }
            finally {
                AdoNet.SqlDisconnect();
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
            string action = null) {

            //System.Threading.Thread.Sleep(2000);

            //** create objects that hold the output data
            List<object> list = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            int acct_id = 0;

            //** hash password
            pwd += ConfigurationManager.AppSettings["mySalt"];
            string moreSalt = BCrypt.Net.BCrypt.GenerateSalt(); // "$2a$10$rBV2JDeWW3.vKyeQcM8fFO"
            string pwdHash = BCrypt.Net.BCrypt.HashPassword(pwd, moreSalt);

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
                        Value = pwdHash,
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
                        Value = Helpers.ConvertToDbNullWhenNull(gender_ix),
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
                dict.Add("ErrMsg", "SQL: " + x.ToString());
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
