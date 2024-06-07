namespace InoxThanhNamServer.Datas.Discount
{
    public class FilterDiscount
    {
        public int? PercentageMin { get; set; }
        public int? PercentageMax { get; set; }
        public string? DateExpire { get; set; }
        public int? LimitedQuantity { get; set; }
        public int? Status { get; set; }
        public string? SortType { get; set; }
        public string? SortFrom { get; set; }
        public string? SearchText { get; set; }
    }
}
