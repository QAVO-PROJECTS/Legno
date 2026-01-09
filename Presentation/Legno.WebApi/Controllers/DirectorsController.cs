using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Director;
using Legno.Application.GlobalExceptionn;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly IDirectorService _directorService;

        public DirectorsController(IDirectorService directorService)
        {
            _directorService = directorService;
        }

        [Authorize(Roles = "Admin")]
        // ✅ Create
        [HttpPost("create-director")]
        public async Task<IActionResult> CreateDirector([FromForm] CreateDirectorDto dto)
        {
            try
            {
                var created = await _directorService.AddDirectorAsync(dto);
                return StatusCode(StatusCodes.Status201Created, new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
        }

        // ✅ Get by id
        [HttpGet("get-director/{directorId}")]
        public async Task<IActionResult> GetDirector(string directorId)
        {
            try
            {
                var director = await _directorService.GetDirectorAsync(directorId);
                return Ok(new { StatusCode = 200, Data = director });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
        }

        // ✅ Get all
        [HttpGet("get-all-directors")]
        public async Task<IActionResult> GetAllDirectors()
        {
            var list = await _directorService.GetAllDirectorsAsync();
            return Ok(new { StatusCode = 200, Data = list });
        }

        [Authorize(Roles = "Admin")]
        // ✅ Update
        [HttpPut("update-director")]
        public async Task<IActionResult> UpdateDirector([FromForm] UpdateDirectorDto dto)
        {
            try
            {
                var updated = await _directorService.UpdateDirectorAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        // ✅ Soft delete
        [HttpDelete("delete-director/{directorId}")]
        public async Task<IActionResult> DeleteDirector(string directorId)
        {
            try
            {
                await _directorService.DeleteDirectorAsync(directorId);
                return Ok(new { StatusCode = 200, Message = "Director silindi." });
            }
            catch (GlobalAppException ex)
            {
                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
        }
    }
}
