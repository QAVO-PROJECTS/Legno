using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Announcement;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementsController(IAnnouncementService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateAnnouncementDto dto)
        {
            try
            {
                var created = await _service.AddAnnouncementAsync(dto);
                return Ok(new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = ex.Message }); }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var item = await _service.GetAnnouncementAsync(id);
                return Ok(new { StatusCode = 200, Data = item });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = ex.Message }); }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllAnnouncementsAsync();
                return Ok(new { StatusCode = 200, Data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = ex.Message }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateAnnouncementDto dto)
        {
            try
            {
                var updated = await _service.UpdateAnnouncementAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = ex.Message }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteAnnouncementAsync(id);
                return Ok(new { StatusCode = 200, Message = "Silindi." });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = ex.Message }); }
        }
    }
}
