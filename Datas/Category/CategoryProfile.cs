namespace InoxThanhNamServer.Datas.Category
{
    public class CategoryProfile
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
