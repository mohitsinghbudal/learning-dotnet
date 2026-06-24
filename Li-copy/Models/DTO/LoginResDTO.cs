namespace Li_copy.Models.DTO
{
    public class LoginResDTO
    {
        public string Token { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public int Id { get; set; } 

        public int RoleId { get; set; }
    }
}
