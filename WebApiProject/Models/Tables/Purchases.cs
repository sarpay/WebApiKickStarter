namespace WebApiProject.Models.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Purchases
    {
        public int ID { get; set; }

        public int AccountID { get; set; }

        public int GoodID { get; set; }
    }
}
