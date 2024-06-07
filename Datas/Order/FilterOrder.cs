namespace InoxThanhNamServer.Datas.Order
{
    public class FilterOrder
    {
        public string? OrderDateFrom { get; set; }
        public string? OrderDateTo { get; set; }
        public double? TotalMin { get; set; }
        public double? TotalMax { get; set; }
        public int? Status { get; set; }
        public bool IsZeroStatus { get; set; } = true;
        public string? SortType { get; set; }
        public string? SortFrom { get; set; }
        public string? SearchText { get; set; }
    }
}
