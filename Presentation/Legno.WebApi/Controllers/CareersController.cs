using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Career;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly ICareerService _service;

        public CareersController(ICareerService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateCareerDto dto)
        {
            try
            {
                var created = await _service.AddCareerAsync(dto);
                return Ok(new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var item = await _service.GetCareerAsync(id);
                return Ok(new { StatusCode = 200, Data = item });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllCareersAsync();
                return Ok(new { StatusCode = 200, Data = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateCareerDto dto)
        {
            try
            {
                var updated = await _service.UpdateCareerAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteCareerAsync(id);
                return Ok(new { StatusCode = 200, Message = "Silindi." });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
    }
}
