namespace InoxThanhNamServer.Datas.UserAddress
{
    public class CreateAddressRequest
    {
        public Guid UserID { get; set; }
        public string? Address { get; set; }
        public int? CityID { get; set; }
        public int? DistrictID { get; set; }
        public int? WardID { get; set; }
    }
}
