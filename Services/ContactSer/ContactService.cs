using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Contact;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.ContactSer
{
    public class ContactService : IContactService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public ContactService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<ContactProfile>> CreateContact(CreateContactRequest request)
        {
            try
            {
                await Task.CompletedTask;
                var contactEntity = _mapper.Map<Contact>(request);
                await _context.Contacts.AddAsync(contactEntity);
                await _context.SaveChangesAsync();
                return new ApiResponse<ContactProfile>
                {
                    Success = true,
                    Message = "Đã gửi tin nhắn của bạn.",
                    Data = _mapper.Map<ContactProfile>(contactEntity),
                    Status = (int)HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ContactProfile>
                {
                    Success = false,
                    Message = "ContactService - CreateContact: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<ContactProfile>>> GetContacts()
        {
            try
            {
                await Task.CompletedTask;
                var contacts = await _context.Contacts.ToListAsync();
                return new ApiResponse<List<ContactProfile>>
                {
                    Success = true,
                    Message = "Lấy danh sách tin nhắn thành công.",
                    Data = contacts.Select(x => _mapper.Map<ContactProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ContactProfile>>
                {
                    Success = false,
                    Message = "ContactService - GetContacts: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
