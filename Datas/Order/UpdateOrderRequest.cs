namespace InoxThanhNamServer.Datas.Order
{
    public class UpdateOrderRequest
    {
        public int OrderID { get; set; }

        public Guid? UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int? ProductQuantity { get; set; }
        public int Status { get; set; }

        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }
}
