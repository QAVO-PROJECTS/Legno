using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.DesignerCommonService;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignerCommonServicesController : ControllerBase
    {
        private readonly IDesignerCommonServiceService _service;
        public DesignerCommonServicesController(IDesignerCommonServiceService service) { _service = service; }

        // Create (CardImage faylı gələ bilər)
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateDesignerCommonServiceDto dto)
        {
            try
            {
                var created = await _service.AddDesignerCommonServiceAsync(dto);
                return CreatedAtAction(nameof(Get), new { designerCommonServiceId = created.Id }, new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [HttpGet("get/{designerCommonServiceId}")]
        public async Task<IActionResult> Get(string designerCommonServiceId)
        {
            try
            {
                var item = await _service.GetDesignerCommonServiceAsync(designerCommonServiceId);
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
                var list = await _service.GetAllCategoriesAsync();
                return Ok(new { StatusCode = 200, Data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        // Update (CardImage yenilənə bilər)
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateDesignerCommonServiceDto dto)
        {
            try
            {
                var updated = await _service.UpdateDesignerCommonServiceAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{designerCommonServiceId}")]
        public async Task<IActionResult> Delete(string designerCommonServiceId)
        {
            try
            {
                await _service.DeleteDesignerCommonServiceAsync(designerCommonServiceId);
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
