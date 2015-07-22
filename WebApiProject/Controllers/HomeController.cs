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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApiProject.Controllers
{
    public class HomeController : ApiController
    {
        private DataModel db = new DataModel();
        protected ADONET AdoNet = new ADONET();

        //*********************************************
        // PUT QUERIES USING STORED PROCEDURES
        //*********************************************

        /*
        [API URI: /new-purchase]
        [VIEW: /views/new-purchase.html]
        DOES NOT REQUIRE PostParameterBinding.cs CLASS TO ACCEPT MULTIPLE PARAMS
        BECAUSE [HttpPut] IS NOT COVERED BY THIS CONFIGURATION (commented out line : 87)
        */
        [Route("new-purchase")]
        [HttpPut]
        public JObject NewPurchase(JObject jsonData)
        {
            //System.Threading.Thread.Sleep(2000);
            
            /** obtain param values from the incoming json object **/
            dynamic newPurchase = jsonData;
            int acct_id = newPurchase.acct_id;
            int good_id = newPurchase.good_id;

            /** create the json object that will hold the output data **/
            dynamic jsonOutput = new JObject();

            try
            {
                /** query the database **/
                AdoNet.SqlConnect();

                /** specify the stored procedure **/
                AdoNet.SqlNewCommand("dbo.newPurchase", "sp");
                /** INs **/
                AdoNet.SqlNewParam("Input", "@AccountID", acct_id, SqlDbType.Int, 0);
                AdoNet.SqlNewParam("Input", "@GoodID", good_id, SqlDbType.Int, 0);
                /** OUTs **/
                AdoNet.SqlNewParam("Output", "@NewID", null, SqlDbType.Int, 0);
                /** Execute SP **/
                AdoNet.SqlExecuteCommand();
                
                /** Obtain output params' values **/
                int newId = 0;
                if (Helpers.TryConvertTo<int>(AdoNet.SqlOutputParamValue("@NewID").ToString()))
                {
                    newId = Convert.ToInt32(AdoNet.SqlOutputParamValue("@NewID").ToString());
                }

                /** populate the output object **/
                jsonOutput.Result = "OK";
                jsonOutput.NewID = newId;
            }
            catch (SqlException x)
            {
                jsonOutput.Result = "ERROR";
                jsonOutput.ErrMsg = "SQL: " + x.ToString();
            }
            catch (Exception x)
            {
                jsonOutput.Result = "ERROR";
                jsonOutput.ErrMsg = "APP: " + x.ToString();
            }
            finally
            {
                AdoNet.SqlDisconnect();
            }

            return jsonOutput;
        }


        //*********************************************
        // POST QUERIES USING STORED PROCEDURES
        //*********************************************

        /*
        [API URI: /sign-in]
        [VIEW: /views/sign-in.html]
        REQUIRES PostParameterBinding.cs CLASS TO ACCEPT MULTIPLE PARAMS
        HttpPost is manipulated by this configuration
        */
        [Route("sign-in")]
        [HttpPost]
        public JObject SignIn(
            string username,
            string password)
        {
            //System.Threading.Thread.Sleep(2000);
            dynamic jsonObject = new JObject();

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
                        // Populate the JSON Object
                        jsonObject.Result = "OK";
                        jsonObject.Ticket = ticket;
                    } else {
                        jsonObject.Result = "BLOCKED";
                        jsonObject.Msg = "Wrong Password!";
                    }
                } else {
                    jsonObject.Result = "BLOCKED";
                    jsonObject.Msg = "No Such User!";
                }
            }
            catch (SqlException x)
            {
                jsonObject.Result = "ERROR";
                jsonObject.ErrMsg = "SQL: " + x.ToString();
            }
            catch (Exception x)
            {
                jsonObject.Result = "ERROR";
                jsonObject.ErrMsg = "APP: " + x.ToString();
            }
            finally
            {
                AdoNet.SqlDisconnect();
            }

            return jsonObject;
        }


        /*
        [API URI: /purchases]
        [VIEW: /views/purchases.html]
        */
        [Route("purchases")]
        [HttpPost]
        //public object[] GetPurchases(
        public JObject GetPurchases(
            int? good_id,
            byte? gender_ix)
        {
            //System.Threading.Thread.Sleep(2000);

            dynamic jsonObject = new JObject();
            //List<object> resultsList = new List<object>();
            //Dictionary<string, object> dict = new Dictionary<string, object>();

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
                    } else if (colDict["ShopperGender"] == "Female") {
                        colDict["ShopperGender"] = "<img src=\"images/female.png\" style=\"width: 20px; height: 20px\" />";
                    } else {
                        colDict["ShopperGender"] = "NULL";
                    }

                }

                // Populate the Dictionary
                jsonObject.Result = "OK";
                jsonObject.Data = JsonConvert.SerializeObject(dataList);
                jsonObject.Count = listCount;

                //dict.Add("Result", "OK");
                //dict.Add("Data", dataList);
                //dict.Add("Array", dataArray);
                //dict.Add("ArrayCount", arrayCount);
                //dict.Add("Count", listCount);
            }
            catch (SqlException x) {
                //dict.Add("Result", "ERROR");
                //dict.Add("ErrMsg", "SQL: " + x.ToString());
                jsonObject.Result = "ERROR";
                jsonObject.ErrMsg = "SQL: " + x.ToString();
            }
            catch (Exception x) {
                //dict.Add("Result", "ERROR");
                //dict.Add("ErrMsg", "APP: " + x.ToString());
                jsonObject.Result = "ERROR";
                jsonObject.ErrMsg = "APP: " + x.ToString();
            }
            finally {
                AdoNet.SqlDisconnect();
            }

            //resultsList.Add(dict);
            //return resultsList.ToArray();
            return jsonObject;
        }


        /*
        [API URI: /new-shopper?action=insert]
        [VIEW: /views/new-shopper.html]
        */
        [Route("new-shopper")]
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

            /** create objects that hold the output data **/
            List<object> list = new List<object>();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            int acct_id = 0;

            /** hash password **/
            pwd += ConfigurationManager.AppSettings["mySalt"];
            string moreSalt = BCrypt.Net.BCrypt.GenerateSalt(); /* looks like : $2a$10$rBV2JDeWW3.vKyeQcM8fFO */
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

                /** obtain output paramater's (@NewID) value **/
                acct_id = (int)params_sp1[0].Value; /** [0] is the index number of the output param **/


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
                dict.Add("Action", action);
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


        /*********************************************/
        /* GET QUERIES USING STORED PROCEDURES */
        /*********************************************/

        /*
        [API URI: /shoppers?gender_ix=1&opt_in=1&_=1111]
        [VIEW: /views/shoppers.html]
        */
        [Route("shoppers")]
        [HttpGet]
        public IEnumerable<getShoppers> GetShoppersFromView(
            byte? gender_ix, 
            byte? opt_in)
        {    
            //try /** DO NOT USE TRY-CATCH HERE SINCE FUNCTION RETURNS THE ENTITY MODEL **/
            //{
            SqlParameter[] sp = {
                new SqlParameter() {
                    ParameterName = "GenderIX",
                    SqlDbType = SqlDbType.TinyInt,
                    Value = Helpers.ConvertToDbNullWhenNull(gender_ix),
                    Direction = ParameterDirection.Input
                },
                new SqlParameter() {
                    ParameterName = "OptIn",
                    SqlDbType = SqlDbType.Bit,
                    Value = Helpers.ConvertToDbNullWhenNull(opt_in),
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


        /*
        [API URI: /shopper/1]
        [VIEW: /views/shopper.html]
        */
        [HttpGet]
        public IEnumerable<getShoppers> GetShopperFromView(
            int id)
        {
            SqlParameter[] sp = {
                new SqlParameter() {
                    ParameterName = "AccountID",
                    SqlDbType = SqlDbType.Int,
                    Value = id,
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

        /*
        [API URI: /genders]
        [VIEW: /views/genders.html]
        */
        [HttpGet]
        public IQueryable<Genders> GetGendersFromTable()
        {
            return db.Genders;
        }


        /*
        [API URI: /gender/{id}]
        [VIEW: /views/gender.html]
        */
        [HttpGet]
        [ResponseType(typeof(Genders))]
        public IHttpActionResult GetGenderFromTable(
            byte id)
        {
            Genders genders = db.Genders.Find(id);
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
