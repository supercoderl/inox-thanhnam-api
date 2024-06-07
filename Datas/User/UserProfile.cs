using InoxThanhNamServer.Datas.UserAddress;

namespace InoxThanhNamServer.Datas.User
{
    public class UserProfile
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int? Phone { get; set; }
        public string? Avatar {  get; set; }

        public AddressProfile? UserAddress { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Roles { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
