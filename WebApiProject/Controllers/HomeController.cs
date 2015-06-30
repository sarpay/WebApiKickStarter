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


        //*********************************************
        // POST QUERIES USING STORED PROCEDURES
        //*********************************************

        // [URI: api/new-shopper?action=insert], [VIEW: new-shopper.html]
        [Route("api/new-shopper")]
        [HttpPost]
        public string NewShopper(
            int acct_id,
            string name,
            byte? gender_ix,
            bool opt_in,
            string action = null)
        {
            SqlParameter[] sp = {
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

            int result = db.Database.ExecuteSqlCommand(
                "dbo.newShopper @AccountID, @Name, @GenderIX, @OptIn", sp);

            return string.Format("Acct. Id: {0}, Name: {1}, Gender: {2}, Opt-In: {3}, Action: {4}", acct_id, name, gender_ix, opt_in, action);
            
            //return new string[] { "value1", "value2" };
            //return result;
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
