using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Legno.Application.Abstracts.Services;   // IBlogService
using Legno.Application.Dtos.Blog;
using Legno.Application.GlobalExceptionn;
using Microsoft.AspNetCore.Authorization;     // GlobalAppException

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // ✅ Yeni blog yarad (fayl yükləmələri üçün FromForm!)
        [Authorize(Roles = "Admin")]
        [HttpPost("create-blog")]
        public async Task<IActionResult> CreateBlog([FromForm] CreateBlogDto dto)
        {
            try
            {
                var created = await _blogService.AddBlogAsync(dto); // service subscriber-lərə mail göndərir
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

        // ✅ Tək blogu ID ilə gətir
        [HttpGet("get-blog/{blogId}")]
        public async Task<IActionResult> GetBlog(string blogId)
        {
            try
            {
                var blog = await _blogService.GetBlogAsync(blogId);
                return Ok(new { StatusCode = 200, Data = blog });
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

        // ✅ Bütün bloglar (ən son əvvəl)
        [HttpGet("get-all-blogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            try
            {
                var blogs = await _blogService.GetAllBlogsAsync();
                return Ok(new { StatusCode = 200, Data = blogs });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
        [Authorize(Roles = "Admin")]
        // ✅ Blog yenilə (fayl yükləmələri üçün FromForm!)
        [HttpPut("update-blog")]
        public async Task<IActionResult> UpdateBlog([FromForm] UpdateBlogDto dto)
        {
            try
            {
                var updated = await _blogService.UpdateBlogAsync(dto);
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
        // ✅ Blog sil (soft delete)
        [HttpDelete("delete-blog/{blogId}")]
        public async Task<IActionResult> DeleteBlog(string blogId)
        {
            try
            {
                await _blogService.DeleteBlogAsync(blogId);
                return Ok(new { StatusCode = 200, Message = "Blog silindi." });
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
