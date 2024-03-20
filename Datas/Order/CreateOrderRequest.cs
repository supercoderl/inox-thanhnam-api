﻿namespace InoxThanhNamServer.Datas.Order
{
    public class CreateOrderRequest
    {
        public Guid? UserID { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; } = 0;

        public int? Status { get; set; } = 0;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}