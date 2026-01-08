using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Purchase;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _service;

        public PurchasesController(IPurchaseService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreatePurchaseDto dto)
        {
            try
            {
                var result = await _service.AddPurchaseAsync(dto);
                return Ok(new { StatusCode = 201, Data = result });
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

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var result = await _service.GetPurchaseAsync(id);
                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (GlobalAppException ex)
            {
                return NotFound(new { StatusCode = 404, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllPurchasesAsync();
                return Ok(new { StatusCode = 200, Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdatePurchaseDtos dto)
        {
            try
            {
                var result = await _service.UpdatePurchaseAsync(dto);
                return Ok(new { StatusCode = 200, Data = result });
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
                await _service.DeletePurchaseAsync(id);
                return Ok(new { StatusCode = 200, Message = "Silindi." });
            }
            catch (GlobalAppException ex)
            {
                return NotFound(new { StatusCode = 404, Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
    }
}
