namespace api_LuanVan.DataTransferObject
{
    public class DTO_User
    {
        public string UserId { get; set; } = null!;

        public string? UPassword { get; set; }

        public string? CustomerName { get; set; }

        public int RolesId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public DateTime? CreateAt { get; set; }
    }
}
