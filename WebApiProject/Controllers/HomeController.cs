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

        //POST: api/new_shopper?action=insert
        [Route("api/newShopper")]
        [HttpPost]
        public string NewShopper(
            int acct_id,
            string name,
            byte? gender_ix,
            bool opt_in,
            string action = null
            )
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
            //return result;
        }

        // GET: api/getShoppers
        [Route("api/getShoppers")]
        public IEnumerable<getShoppers> GetShoppersFromView()
        {
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

            //return db.Database.SqlQuery<Shoppers_Result>(
            //    "dbo.getShoppers @GenderIX, @OptIn", sp)
            //    .ToList();

            // ---------- OR ----------

            // This REQUIRES Shoppers_Result DbSet DEFINED IN DataModel.cs
            return db.getShoppers.SqlQuery(
                "dbo.getShoppers @GenderIX, @OptIn", sp)
                .ToList();

            //return new string[] { "value1", "value2" };
        }


        // GET: api/Shoppers
        [Route("api/Shoppers")]
        public IQueryable<Shoppers> GetShoppersFromTable()
        {
            return db.Shoppers;
        }


        // GET: api/getShoppers/1
        [Route("api/getShoppers/{acct_id:int}")]
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

            var results = db.getShoppers.SqlQuery(
                "dbo.getShopper @AccountID", sp)
                .ToList();

            return results;
        }


        // GET: api/Shoppers/1
        [Route("api/Shoppers/{acct_id:int}")]
        [ResponseType(typeof(Shoppers))]
        public IHttpActionResult GetShopperFromTable(int acct_id)
        {
            Shoppers shoppers = db.Shoppers.Find(acct_id);
            if (shoppers == null)
            {
                return NotFound();
            }
            return Ok(shoppers);
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
