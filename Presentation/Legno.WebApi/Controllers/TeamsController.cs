using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Legno.Application.Absrtacts.Services; // sənin nümunəndə bu namespace işlənib
using Legno.Application.Dtos.Team;
using Legno.Application.GlobalExceptionn;   // sənin nümunəndə bu adla istifadə olunub
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Legno.Application.Abstracts.Services;
using Microsoft.AspNetCore.Authorization;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }
        [Authorize(Roles = "Admin")]
        // ✅ Yeni team üzvü yarat
        [HttpPost("create-team")]
        public async Task<IActionResult> CreateTeam([FromForm] CreateTeamDto dto)
        {
            try
            {
                var created = await _teamService.AddTeamAsync(dto);
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

        // ✅ Tək team üzvünü ID ilə gətir
        [HttpGet("get-team/{teamId}")]
        public async Task<IActionResult> GetTeam(string teamId)
        {
            try
            {
                var team = await _teamService.GetTeamAsync(teamId);
                return Ok(new { StatusCode = 200, Data = team });
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

        // ✅ Bütün team üzvlərini gətir (DisplayOrderId ilə sıralanmış)
        [HttpGet("get-all-teams")]
        public async Task<IActionResult> GetAllTeams()
        {
            try
            {
                var teams = await _teamService.GetAllTeamsAsync();
                return Ok(new { StatusCode = 200, Data = teams });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Admin")]
        // ✅ Team üzvünü yenilə (UpdateTeamDto daxilində Id olmalıdır)
        [HttpPut("update-team")]
        public async Task<IActionResult> UpdateTeam([FromForm] UpdateTeamDto dto)
        {
            try
            {
                var updated = await _teamService.UpdateTeamAsync(dto);
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
        // ✅ Team üzvünü sil (soft delete)
        [HttpDelete("delete-team/{teamId}")]
        public async Task<IActionResult> DeleteTeam(string teamId)
        {
            try
            {
                await _teamService.DeleteTeamAsync(teamId);
                return Ok(new { StatusCode = 200, Message = "Team üzvü silindi." });
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

        // ✅ Toplu reorder: TeamId + DisplayOrderId siyahısı ilə sıralama dəyiş
        [HttpPut("reorder-teams")]
        public async Task<IActionResult> ReorderTeams([FromBody] List<TeamOrderUpdateDto> orders)
        {
            try
            {
                await _teamService.ReorderTeamsAsync(orders);
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
    }
}
