using Microsoft.AspNetCore.Mvc;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Dtos.Contact;

using Legno.Application.GlobalExceptionn;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // ✅ Yeni kontakt (istifadəçi) məlumatı yarat
        [HttpPost("create-contact")]
        public async Task<IActionResult> CreateContact([FromBody] CreateContactDto createContactDto)
        {
            try
            {
                var contact = await _contactService.CreateUserInfoAsync(createContactDto);
                return StatusCode(StatusCodes.Status201Created, new { StatusCode = 201, Data = contact });
            }
            catch (GlobalAppException ex)
            {
                // Uygulama içi fırlatılan özel hatalar (örnek: validasyon, iş kuralı ihlali)
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex)
            {
                // Beklenmeyen hatalar
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
        [Authorize(Roles = "Admin")]
        // ✅ Bütün kontaktları (istifadəçiləri) getir
        [HttpGet("get-all-contacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            try
            {
                var contacts = await _contactService.GetAllUsersAsync();
                return Ok(new { StatusCode = 200, Data = contacts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("contact/{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            try
            {
                var contacts = await _contactService.GetByIdAsync(id);
                return Ok(new { StatusCode = 200, Data = contacts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
    }
}
