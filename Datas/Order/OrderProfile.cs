namespace InoxThanhNamServer.Datas.Order
{
    public class OrderProfile
    {
        public int OrderID { get; set; }
        public Guid? UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
