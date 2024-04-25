using _2World.Data;
using _2World.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace _2World.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext context;

        public HomeController(ILogger<HomeController> logger, AppDbContext _context)
        {
            _logger = logger;
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            int month = DateTime.Now.Month;
            ViewBag.TotalRevenueInMonth = GetTotalRevenueInMonth(month);
            int year = DateTime.Now.Year;
            ViewBag.TotalRevenueInYear = GetTotalRevenueInYear(year);
            ViewBag.TotalPendingOrdersAmount = GetTotalPendingOrders();
            IQueryable<Product> topProducts = GetTopBestSellingProductsOfMonth(year, month, 5);
            return View(topProducts);
        }

        public decimal GetTotalRevenueInMonth(int month)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            decimal totalRevenue = context.Orders
                .Join(context.OrderItems,
                    order => order.Id,
                    orderItem => orderItem.Order_Id,
                    (order, orderItem) => new { Order = order, OrderItem = orderItem })
                .Where(joinResult => joinResult.Order.Status == 2 && joinResult.Order.Order_Date >= startDate && joinResult.Order.Order_Date <= endDate)
                .Sum(joinResult => joinResult.OrderItem.Price * joinResult.OrderItem.Quantity);

            return totalRevenue;
        }

        public decimal GetTotalRevenueInYear(int year)
        {
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, 31);

            decimal totalRevenue = context.Orders
                .Where(order => order.Status == 2 && order.Order_Date >= startDate && order.Order_Date <= endDate)
                .Join(context.OrderItems,
                    order => order.Id,
                    orderItem => orderItem.Order_Id,
                    (order, orderItem) => orderItem.Price * orderItem.Quantity)
                .Sum();

            return totalRevenue;
        }

        public decimal GetTotalPendingOrders()
        {
            decimal totalOrder = context.Orders
                .Where(order => order.Status == 0)
                .Count();

            return totalOrder;
        }

        public IQueryable<Product> GetTopBestSellingProductsOfMonth(int year, int month, int limit = 5)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            var topSellingProducts = context.OrderItems
                .Where(orderItem => orderItem.Order.Order_Date >= startDate && orderItem.Order.Order_Date <= endDate)
                .GroupBy(orderItem => orderItem.Product_Id)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalQuantity = group.Sum(orderItem => orderItem.Quantity)
                })
                .OrderByDescending(result => result.TotalQuantity)
                .Take(limit)
                .Join(context.Products, result => result.ProductId, product => product.Id, (result, product) => product);

            return topSellingProducts;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
