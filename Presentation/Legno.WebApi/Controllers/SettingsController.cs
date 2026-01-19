using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Setting;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _service;

        public SettingsController(ISettingService service)
        {
            _service = service;
        }

        // ================= CREATE =================
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateSettingDto dto)
        {
            try
            {
                var created = await _service.AddSettingAsync(dto);
                return StatusCode(StatusCodes.Status201Created, new { StatusCode = 201, Data = created });
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

        // ================= GET ONE =================
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var setting = await _service.GetSettingAsync(id);

                if (setting == null)
                    return NotFound(new { StatusCode = 404, Error = "Setting tapılmadı!" });

                return Ok(new { StatusCode = 200, Data = setting });
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

        // ================= GET ALL =================
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllSettingsAsync();
                return Ok(new { StatusCode = 200, Data = list });
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

        // ================= UPDATE =================
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateSettingDto dto)
        {
            try
            {
                ApplyEmptyStrings(dto);

                var updated = await _service.UpdateSettingAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
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

        // ================= DELETE =================
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteSettingAsync(id);
                return Ok(new { StatusCode = 200, Message = "Setting silindi." });
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

        // ================= HELPERS =================

        private void ApplyEmptyStrings(UpdateSettingDto dto)
        {
            var form = Request.Form;

            dto.Key = FixEmpty(form, "Key", dto.Key);
            dto.Name = FixEmpty(form, "Name", dto.Name);
            dto.Value = FixEmpty(form, "Value", dto.Value);
            dto.ValueEng = FixEmpty(form, "ValueEng", dto.ValueEng);
            dto.ValueRu = FixEmpty(form, "ValueRu", dto.ValueRu);
        }

        private string? FixEmpty(IFormCollection form, string key, string? currentValue)
        {
            if (!form.ContainsKey(key))
                return currentValue;

            var value = form[key].ToString();

            if (string.IsNullOrWhiteSpace(value))
                return "";

            return value;
        }
    }
}
