namespace WebApiProject.Models.Views
{
    using System;

    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class getPurchases
    {
        [Key]
        public int ID { get; set; }
        public string GoodName { get; set; }
        public decimal GoodPrice { get; set; }
        public string ShopperEmail { get; set; }
        public string ShopperName { get; set; }
        public string ShopperGender { get; set; }
    }
}