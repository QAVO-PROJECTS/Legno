using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Member;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service)
        {
            _service = service;
        }

        // ================= CREATE =================
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateMemberDto dto)
        {
            try
            {
                var created = await _service.AddMemberAsync(dto);
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
                var member = await _service.GetMemberAsync(id);

                if (member == null)
                    return NotFound(new { StatusCode = 404, Error = "Member tapılmadı!" });

                return Ok(new { StatusCode = 200, Data = member });
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
                var list = await _service.GetAllMembersAsync();
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
        public async Task<IActionResult> Update([FromForm] UpdateMemberDto dto)
        {
            try
            {
                // ✅ Swagger form-data boş dəyərləri bəzən göndərmir
                // ✅ Göndəribsə, amma boşdursa => "" eləyirik
                ApplyEmptyStrings(dto);

                var updated = await _service.UpdateMemberAsync(dto);
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

        // ================= REORDER =================
        [Authorize(Roles = "Admin")]
        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder([FromBody] List<MemberOrderUpdateDto> orders)
        {
            try
            {
                await _service.ReorderMembersAsync(orders);
                return Ok(new { StatusCode = 200, Message = "Sıralama yeniləndi." });
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

        // ================= DELETE =================
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteMemberAsync(id);
                return Ok(new { StatusCode = 200, Message = "Üzv silindi." });
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

        private void ApplyEmptyStrings(UpdateMemberDto dto)
        {
            var form = Request.Form;

            // ⚠️ Burada key adları UpdateMemberDto-nun property adları ilə
            // tam eyni olmalıdır (Swagger/Postman belə göndərir)

      
            dto.TitleEng = FixEmpty(form, "TitleEng", dto.TitleEng);
            dto.TitleRu = FixEmpty(form, "TitleRu", dto.TitleRu);

            dto.JobTitle = FixEmpty(form, "JobTitle", dto.JobTitle);
            dto.JobTitleEng = FixEmpty(form, "JobTitleEng", dto.JobTitleEng);
            dto.JobTitleRu = FixEmpty(form, "JobTitleRu", dto.JobTitleRu);

            dto.InstagramLink = FixEmpty(form, "InstagramLink", dto.InstagramLink);
        }

        private string? FixEmpty(IFormCollection form, string key, string? currentValue)
        {
            // field ümumiyyətlə gəlməyibsə => null qalsın => əvvəlki dəyər DB-də qalacaq
            if (!form.ContainsKey(key))
                return currentValue;

            var value = form[key].ToString();

            // field gəlibsə amma boşdursa / boşluqdursa => DB-də empty string olsun
            if (string.IsNullOrWhiteSpace(value))
                return "";

            return value;
        }
    }
}
