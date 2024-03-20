namespace InoxThanhNamServer.Datas.UserAddress
{
    public class AddressProfile
    {
        public int AddressID { get; set; }
        public Guid UserID { get; set; }
        public string? Address {  get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
        public int WardID { get; set; }
    }
}
