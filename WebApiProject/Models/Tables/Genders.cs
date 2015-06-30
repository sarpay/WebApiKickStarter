namespace WebApiProject.Models.Tables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Genders
    {
        [Key]
        public byte IX { get; set; }

        [Required]
        [StringLength(6)]
        public string Text { get; set; }
    }
}
