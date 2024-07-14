using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2World.Models
{
    public class OrderItem
    {
        [Key, Column(Order = 0)]
        public int Order_Id { get; set; }
        [ForeignKey("Id")]
        public Order? Order { get; set; }
        [Key, Column(Order = 1)]
        public int Product_Id { get; set; }
        [ForeignKey("Id")]
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

    }
}
