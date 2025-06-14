namespace api_LuanVan.DataTransferObject
{
    public class DTO_OrderTable
    {
        public long OrderTableId { get; set; }

        public string? UserId { get; set; }

        public DateTime StartingTime { get; set; }

        public bool isCancel { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? TotalDeposit { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
