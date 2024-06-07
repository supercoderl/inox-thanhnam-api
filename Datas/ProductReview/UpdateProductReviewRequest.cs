namespace InoxThanhNamServer.Datas.ProductReview
{
    public class UpdateProductReviewRequest
    {
        public int ReviewID { get; set; }
        public int? Likes { get; set; }
        public int? Unlikes { get; set; }
    }
}
