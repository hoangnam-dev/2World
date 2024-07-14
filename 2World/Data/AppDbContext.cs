using _2World.Models;
using Microsoft.EntityFrameworkCore;

namespace _2World.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.Category_Id);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.Role_Id);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,0)");

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.Price)
                .HasColumnType("decimal(18,0)");

            modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.Role_Id, rp.Permission_Id });

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.Role_Id);

            modelBuilder.Entity<Role>()
                .HasMany(r => r.RolePermissions)
                .WithOne(rp => rp.Role)
                .HasForeignKey(rp => rp.Role_Id);

            modelBuilder.Entity<Permission>()
                .HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.Permission_Id);

            modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.Customer_Id);

            modelBuilder.Entity<OrderItem>()
            .HasKey(oi => new { oi.Order_Id, oi.Product_Id });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.Order_Id);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.Product_Id);

            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Order>().ToTable("Order");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Permission>().ToTable("Permission");

            modelBuilder.Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "ADMIN",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "USER",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "GUEST",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    }
                );
            modelBuilder.Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        Name = "Admin",
                        Email = "admin@gmail.com",
                        Phone = "0987654321",
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Role_Id = 1,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    }
                );
            Random random = new Random();
            modelBuilder.Entity<Customer>()
                .HasData(
                    new Customer
                    {
                        Id = 1,
                        Name = Faker.Name.FullName(),
                        Email = Faker.Name.First() + "@gmail.com",
                        Phone = Faker.Phone.Number(),
                        Address = Faker.Address.City(),
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Customer
                    {
                        Id = 2,
                        Name = Faker.Name.FullName(),
                        Email = Faker.Name.First() + "@gmail.com",
                        Phone = Faker.Phone.Number(),
                        Address = Faker.Address.City(),
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Customer
                    {
                        Id = 3,
                        Name = Faker.Name.FullName(),
                        Email = Faker.Name.First() + "@gmail.com",
                        Phone = Faker.Phone.Number(),
                        Address = Faker.Address.City(),
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Customer
                    {
                        Id = 4,
                        Name = Faker.Name.FullName(),
                        Email = Faker.Name.First() + "@gmail.com",
                        Phone = Faker.Phone.Number(),
                        Address = Faker.Address.City(),
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Customer
                    {
                        Id = 5,
                        Name = Faker.Name.FullName(),
                        Email = Faker.Name.First() + "@gmail.com",
                        Phone = Faker.Phone.Number(),
                        Address = Faker.Address.City(),
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword("password", 13),
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    }
                );
            modelBuilder.Entity<Category>()
                .HasData(
                    new Category
                    {
                        Id = 1,
                        Name = "Books",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Category
                    {
                        Id = 2,
                        Name = "Electronics",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Category
                    {
                        Id = 3,
                        Name = "Clothes",
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    }
                );
            modelBuilder.Entity<Product>()
                .HasData(
                    new Product
                    {
                        Id = 1,
                        Name = "Product 1",
                        Price = 10,
                        Quantity = 12,
                        Category_Id = 1,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "Product 2",
                        Price = 12,
                        Quantity = 12,
                        Category_Id = 2,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "Product 3",
                        Price = 13,
                        Quantity = 12,
                        Category_Id = 1,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 4,
                        Name = "Product 4",
                        Price = 14,
                        Quantity = 12,
                        Category_Id = 3,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 5,
                        Name = "Product 5",
                        Price = 15,
                        Quantity = 12,
                        Category_Id = 3,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 6,
                        Name = "Product 6",
                        Price = 16,
                        Quantity = 12,
                        Category_Id = 1,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 7,
                        Name = "Product 7",
                        Price = 17,
                        Quantity = 12,
                        Category_Id = 2,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 8,
                        Name = "Product 8",
                        Price = 18,
                        Quantity = 12,
                        Category_Id = 2,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 9,
                        Name = "Product 9",
                        Price = 19,
                        Quantity = 12,
                        Category_Id = 1,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    },
                    new Product
                    {
                        Id = 10,
                        Name = "Product 10",
                        Price = 20,
                        Quantity = 12,
                        Category_Id = 3,
                        Created_At = DateTime.Now,
                        Updated_At = DateTime.Now,
                    }
                );
            modelBuilder.Entity<Order>()
                .HasData(
                    new Order
                    {
                        Id = 1,
                        Customer_Id = 1,
                        Order_Date = new DateTime(2024, 02, 21),
                        Delivery_Date = new DateTime(2024, 03, 16),
                        Status = 1,
                    },
                    new Order
                    {
                        Id = 2,
                        Customer_Id = 2,
                        Order_Date = new DateTime(2024, 03, 01),
                        Delivery_Date = new DateTime(2024, 03, 31),
                        Status = 2,
                    },
                    new Order
                    {
                        Id = 3,
                        Customer_Id = 2,
                        Order_Date = new DateTime(2024, 04, 29),
                        Delivery_Date = new DateTime(2024, 05, 07),
                        Status = 2,
                    },
                    new Order
                    {
                        Id = 4,
                        Customer_Id = 5,
                        Order_Date = new DateTime(2024, 04, 04),
                        Status = 0,
                    },
                    new Order
                    {
                        Id = 5,
                        Customer_Id = 4,
                        Order_Date = new DateTime(2024, 03, 31),
                        Status = 0,
                    },
                    new Order
                    {
                        Id = 6,
                        Customer_Id = 3,
                        Order_Date = new DateTime(2023, 09, 23),
                        Delivery_Date = new DateTime(2023, 10, 04),
                        Status = 1,
                    }
                );
            modelBuilder.Entity<OrderItem>()
                .HasData(
                    new OrderItem
                    {
                        Order_Id = 1,
                        Product_Id = 1,
                        Price = 11,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 2,
                        Product_Id = 1,
                        Price = 11,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 2,
                        Product_Id = 2,
                        Price = 12,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 3,
                        Product_Id = 4,
                        Price = 14,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 4,
                        Product_Id = 1,
                        Price = 11,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 4,
                        Product_Id = 2,
                        Price = 12,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 4,
                        Product_Id = 4,
                        Price = 14,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 5,
                        Product_Id = 3,
                        Price = 13,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 6,
                        Product_Id = 1,
                        Price = 11,
                        Quantity = 1
                    },
                    new OrderItem
                    {
                        Order_Id = 6,
                        Product_Id = 10,
                        Price = 20,
                        Quantity = 1
                    }
                );
        }

    }
}
