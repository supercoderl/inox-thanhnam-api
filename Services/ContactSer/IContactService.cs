using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Contact;

namespace InoxThanhNamServer.Services.ContactSer
{
    public interface IContactService
    {
        Task<ApiResponse<ContactProfile>> CreateContact(CreateContactRequest request);
        Task<ApiResponse<List<ContactProfile>>> GetContacts(string? text);
    }
}
