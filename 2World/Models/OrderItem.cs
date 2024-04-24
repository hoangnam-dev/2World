using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2World.Models
{
    public class OrderItem
    {

        public int Order_Id { get; set; }
        [ForeignKey("Id")]
        public Order? Order { get; set; }
        public int Product_Id { get; set; }
        [ForeignKey("Id")]
        public Product? Product { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }


    }
}
