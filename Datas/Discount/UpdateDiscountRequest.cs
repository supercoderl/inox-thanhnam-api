namespace InoxThanhNamServer.Datas.Discount
{
    public class UpdateDiscountRequest
    {
        public int DiscountID { get; set; }
        public string? Name { get; set; }

        public string? Code { get; set; }
        public int Percentage { get; set; }
        public string? DateExpire { get; set; }
        public int LimitedQuantity { get; set; }
        public bool Active { get; set; }
        public int? Priority { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
