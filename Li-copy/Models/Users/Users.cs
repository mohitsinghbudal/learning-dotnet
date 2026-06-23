namespace Li_copy.Models.Users
{
    public class User
    {
        //public int Id { get; set; }
        //public string FullName { get; set; } = string.Empty;

        //public string Email { get; set; } = string.Empty;


        //public string PasswordHash { get; set; } = string.Empty;

        //public decimal?  Phone { get; set; }

        //public int RoleId { get; set; }

        //public DateTime CreatedAt { get; set; }

        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
