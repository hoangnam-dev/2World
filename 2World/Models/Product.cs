using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2World.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [Display(Name ="Product name")]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Product price")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Product quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Product avatar")]
        public string? Image_Path { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Created_At { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Updated_At { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Deleted_At { get; set; }
        [Required]
        public int Category_Id { get; set; }
        [BindNever]
        [ForeignKey("Id")]
        public Category? Category { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
