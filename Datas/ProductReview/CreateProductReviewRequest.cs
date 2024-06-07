namespace InoxThanhNamServer.Datas.ProductReview
{
    public class CreateProductReviewRequest
    {
        public int ProductReviewID { get; set; }
        public int ProductID { get; set; }
        public string? ReviewerName { get; set; }
        public string? ReviewContent { get; set; }
        public int? Rating { get; set; }
        public Guid? UserID { get; set; }
        public Guid? SessionID { get; set; }
        public DateTime? ReviewDate { get; set; } = DateTime.Now;
    }
}
