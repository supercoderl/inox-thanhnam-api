namespace InoxThanhNamServer.Datas.Product
{
    public class ProductProfile
    {
        public int ProductID { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public int? CategoryID { get; set; }

        public int? Quantity { get; set; }
        public double Price { get; set; }
        public int Priority { get; set; }
        public int? DiscountID { get; set; }
        public string Material { get; set; }
        public string Dimension { get; set; }
        public string Origin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string? ImageURL { get; set; }
    }
}
