namespace _2World.Models.ViewModels
{
    public class OrderItemViewModel
    {
        public int Order_Id { get; set; }
        public int Product_Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
