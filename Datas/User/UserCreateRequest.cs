namespace InoxThanhNamServer.Datas.User
{
    public class UserCreateRequest
    {
        public Guid UserID { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public bool? IsActive { get; set; } = true;
        public string Phone { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
