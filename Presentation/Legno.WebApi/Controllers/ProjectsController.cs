using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Project;
using Legno.Application.GlobalExceptionn;
using Legno.Persistence.Concreters.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;
        public ProjectsController(IProjectService service) { _service = service; }

        // Create (FromForm — şəkil/video faylları gəlir)
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateProjectDto dto)
        {
            try
            {
                var created = await _service.AddProjectAsync(dto);
                return StatusCode(StatusCodes.Status201Created, new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var item = await _service.GetProjectAsync(id);
                return Ok(new { StatusCode = 200, Data = item });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllProjectsAsync();
                return Ok(new { StatusCode = 200, Data = list });
            }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }

        // Update (FromForm — yenidən fayl gələ bilər)
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateProjectDto dto)
        {
            try
            {
                var updated = await _service.UpdateProjectAsync(dto);
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
                await _service.DeleteProjectAsync(id);
                return Ok(new { StatusCode = 200, Message = "Silindi." });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }
        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory([FromRoute] string categoryId)
        {
            try
            {
                var projects = await _service.GetProjectsByCategoryAsync(categoryId);
                return Ok(new { StatusCode = 200, Data = projects });
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
        // Kütləvi reorder
        [Authorize(Roles = "Admin")]
        [HttpPost("reorder")]
        public async Task<IActionResult> Reorder([FromBody] List<ProjectReorderDto> items)
        {
            try
            {
                await _service.BulkReorderAsync(items);
                return Ok(new { StatusCode = 200, Message = "Sıra yeniləndi." });
            }
            catch (GlobalAppException ex) { return BadRequest(new { StatusCode = 400, Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" }); }
        }
    }
}
