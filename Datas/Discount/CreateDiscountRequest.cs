namespace InoxThanhNamServer.Datas.Discount
{
    public class CreateDiscountRequest
    {
        public string? Name { get; set; }

        public string? Code { get; set; }
        public int Percentage { get; set; }
        public string? DateExpire { get; set; }
        public int LimitedQuantity { get; set; } = 0;
        public bool? Active { get; set; } = true;
        public int? Priority { get; set; } = 0;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
