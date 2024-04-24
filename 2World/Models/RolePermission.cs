namespace _2World.Models
{
    public class RolePermission
    {
        public int Role_Id { get; set; }
        public Role? Role { get; set; }
        public int Permission_Id { get; set; }
        public Permission? Permission { get; set; }
    }
}
