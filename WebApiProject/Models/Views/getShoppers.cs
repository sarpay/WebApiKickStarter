namespace WebApiProject.Models.Views
{
    using System;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class getShoppers
    {
        [Key]
        public int ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public bool OptIn { get; set; }
        public DateTime RegDate { get; set; }
    }
}