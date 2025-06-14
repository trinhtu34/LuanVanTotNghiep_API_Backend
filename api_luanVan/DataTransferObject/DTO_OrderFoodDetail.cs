namespace api_LuanVan.DataTransferObject
{
    public class DTO_OrderFoodDetail
    {
        public int OrderFoodDetailsId { get; set; }

        public long OrderTableId { get; set; }

        public string DishId { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public string? Note { get; set; }
    }
}
