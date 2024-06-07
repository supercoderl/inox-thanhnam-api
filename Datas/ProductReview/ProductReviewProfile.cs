namespace InoxThanhNamServer.Datas.ProductReview
{
    public class ProductReviewProfile
    {
        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public string? ReviewerName { get; set; }
        public string? ReviewContent { get; set; }
        public int? Rating { get; set; }
        public int? Likes { get; set; }
        public int? Unlikes { get; set; }
        public Guid? UserID { get; set; }
        public Guid? SessionID { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
