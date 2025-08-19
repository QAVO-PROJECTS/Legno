using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Legno.Application.Abstracts.Services;   // ISubscriberService buradadır
using Legno.Application.GlobalExceptionn;     // Səndə bu işlənirdi; fərqlidirsə GlobalException istifadə et
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Legno.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly ISubscriberService _subscriberService;

        public SubscribersController(ISubscriberService subscriberService)
        {
            _subscriberService = subscriberService;
        }

        // Sadə request modeli (email üçün)
        public class AddSubscriberRequest
        {
            public string Email { get; set; }
        }

        // ✅ Yeni abunəçi əlavə et
        [HttpPost("create-subscriber")]
        public async Task<IActionResult> CreateSubscriber([FromBody] AddSubscriberRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(new { StatusCode = 400, Error = "Email tələb olunur." });

                await _subscriberService.AddSubscriberAsync(request.Email);
                return StatusCode(StatusCodes.Status201Created, new { StatusCode = 201, Message = "Abunəçi əlavə olundu." });
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
        // ✅ Tək abunəçini ID ilə gətir (email qaytarır)
        [HttpGet("get-subscriber/{subscriberId}")]
        public async Task<IActionResult> GetSubscriber(string subscriberId)
        {
            try
            {
                var email = await _subscriberService.GetSubscriberAsync(subscriberId);
                return Ok(new { StatusCode = 200, Data = email });
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
        // ✅ Bütün abunəçilərin email-larını gətir
        [HttpGet("get-all-subscribers")]
        public async Task<IActionResult> GetAllSubscribers()
        {
            try
            {
                var emails = await _subscriberService.GetAllSubscribersAsync();
                return Ok(new { StatusCode = 200, Data = emails });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Error = $"Xəta baş verdi: {ex.Message}" });
            }
        }
        [Authorize(Roles = "Admin")]
        // ✅ Abunəçini sil (soft delete)
        [HttpDelete("delete-subscriber/{subscriberId}")]
        public async Task<IActionResult> DeleteSubscriber(string subscriberId)
        {
            try
            {
                await _subscriberService.DeleteSubscriberAsync(subscriberId);
                return Ok(new { StatusCode = 200, Message = "Abunəçi silindi." });
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
