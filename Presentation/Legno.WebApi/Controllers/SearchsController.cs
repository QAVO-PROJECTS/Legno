// Legno.Api/Controllers/SearchController.cs
using Legno.Application.Abstracts.Services;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IFuzzyProjectSearchService _search;

        public SearchController(IFuzzyProjectSearchService search)
        {
            _search = search;
        }

        // GET: /api/search/projects?q=park&take=20
        [HttpGet("projects")]
        public async Task<IActionResult> SearchProjects([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest(new { StatusCode = 400, Error = "Sorğu boş ola bilməz." });

                var data = await _search.SearchAsync(q);
                return Ok(new { StatusCode = 200, Data = data });
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
