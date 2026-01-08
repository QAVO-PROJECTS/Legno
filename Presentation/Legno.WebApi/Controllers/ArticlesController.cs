using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Article;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticlesController(IArticleService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateArticleDto dto)
        {
            try
            {
                var created = await _service.AddArticleAsync(dto);
                return Ok(new { StatusCode = 201, Data = created });
            }
            catch (GlobalAppException ex) { return BadRequest(new { Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Error = ex.Message }); }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var item = await _service.GetArticleAsync(id);
                return Ok(new { StatusCode=201, Data = item });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new {StatusCode=404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { Error = ex.Message }); }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _service.GetAllArticlesAsync();
                return Ok(new { StatusCode=200,Data = list });
            }
               catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });

                return BadRequest(new { StatusCode = 400, Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { Error = ex.Message }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateArticleDto dto)
        {
            try
            {
                var updated = await _service.UpdateArticleAsync(dto);
                return Ok(new { StatusCode = 200, Data = updated });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404, Error = ex.Message });
                return BadRequest(new { StatusCode = 400,Error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { Error = ex.Message }); }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteArticleAsync(id);
                return Ok(new { StatusCode = 200,message = "Məqalə Silindi." });
            }
            catch (GlobalAppException ex)
            {
                if (ex.Message.Contains("tapılmadı", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { StatusCode = 404,Error = ex.Message });

                return BadRequest(new { StatusCode = 400,Error = ex.Message });
            }
            catch (Exception ex) { return StatusCode(500, new { Error = ex.Message }); }
        }
    }
}
