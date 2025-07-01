namespace api_LuanVan.DataTransferObject
{
    public class DTO_Cart
    {
        public long CartId { get; set; }

        public string UserId { get; set; } = null!;

        public DateTime? OrderTime { get; set; }
        public decimal TotalPrice { get; set; }

        public bool? IsCancel { get; set; }
    }
}
