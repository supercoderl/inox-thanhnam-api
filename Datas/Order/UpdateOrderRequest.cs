namespace InoxThanhNamServer.Datas.Order
{
    public class UpdateOrderRequest
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }
    }
}
