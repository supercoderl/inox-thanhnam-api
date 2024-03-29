namespace InoxThanhNamServer.Datas.Order
{
    public class FilterOrder
    {
        public string? Text { get; set; }
        public string? Status { get; set; }

         public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }
    }
}
