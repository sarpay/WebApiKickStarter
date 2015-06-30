namespace WebApiProject.Models.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Goods
    {
        public int ID { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "smallmoney")]
        public decimal? Price { get; set; }
    }
}
