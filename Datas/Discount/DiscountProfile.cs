namespace InoxThanhNamServer.Datas.Discount
{
    public class DiscountProfile
    {
        public int DiscountID { get; set; }
        public string? Name { get; set; }

        public string? Code { get; set; }
        public int Percentage { get; set; }
        public bool Active { get; set; }
        public int? Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
