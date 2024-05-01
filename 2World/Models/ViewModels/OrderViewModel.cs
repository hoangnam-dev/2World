using System.ComponentModel.DataAnnotations;

namespace _2World.Models.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime Order_Date { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Delivery_Date { get; set; }
        public int Status { get; set; }
        public decimal? TotalAmount { get; set; } // Tổng số tiền của đơn hàng

        // Danh sách các mặt hàng trong đơn hàng
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
