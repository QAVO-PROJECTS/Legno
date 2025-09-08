using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {

        private readonly IPartnerService _service;
        public PartnersController(IPartnerService service) { _service = service; }
        //[Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateBusinessServiceDto dto)
        {
            try
            {
                var created = await _service.AddBusinessServiceAsync(dto);
                return Ok(new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var item = await _service.GetBusinessServiceAsync(id);
                return Ok(new { StatusCode = 200, Data = item });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllBusinessServicesAsync();
                return Ok(new { StatusCode = 200, Data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateBusinessServiceDto dto)
        {
            try
            {
                var updated = await _service.UpdateBusinessServiceAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteBusinessServiceAsync(id);
                return Ok(new { StatusCode = 200, Message = "Silindi." });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }
    }
}
