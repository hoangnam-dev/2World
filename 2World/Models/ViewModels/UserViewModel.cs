using System.ComponentModel.DataAnnotations;

namespace _2World.Models.ViewModels
{
    public class UserViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Password { get; set; }
        [Required]
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
