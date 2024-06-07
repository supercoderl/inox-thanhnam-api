namespace InoxThanhNamServer.Datas.Product
{
    public class FilterProduct
    {
        public string? UpdatedDateFrom { get; set; }
        public string? UpdatedDateTo { get; set; }
        public double? PriceMin { get; set; }
        public double? PriceMax { get; set; }
        public int? CategoryID { get; set; }
        public int? ProductID { get; set; }
        public string? SortType { get; set; }
        public string? SortFrom { get; set; }
        public string? SearchText { get; set; }
    }
}
