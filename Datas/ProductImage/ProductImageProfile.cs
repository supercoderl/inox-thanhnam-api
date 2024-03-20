﻿namespace InoxThanhNamServer.Datas.ProductImage
{
    public class ProductImageProfile
    {
        public int ImageID { get; set; }
        public string ImageName { get; set; }
        public string ImageURL { get; set; }
        public int ProductID { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
