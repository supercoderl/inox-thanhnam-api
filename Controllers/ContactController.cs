using InoxThanhNamServer.Datas.Contact;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Services.ContactSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("contacts")]
        public async Task<IActionResult> GetContacts(string? text)
        {
            var result = await _contactService.GetContacts(text);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-contact")]
        public async Task<IActionResult> CreateContact(CreateContactRequest request)
        {
            var result = await _contactService.CreateContact(request);
            return StatusCode(result.Status, result);
        }
    }
}
