﻿namespace InoxThanhNamServer.Datas.Contact
{
    public class ContactProfile
    {
        public int ContactID { get; set; }
        public string Fullname { get; set; }
        public int Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
