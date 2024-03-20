namespace InoxThanhNamServer.Datas.Order
{
    public class CreateOrderItemRequest
    {
        public int? OrderID { get; set; }
        public int? ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
