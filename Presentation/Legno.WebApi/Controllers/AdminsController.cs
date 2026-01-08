using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Dtos.Account;
using Legno.Application.GlobalExceptionn;

namespace Legno.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminsController> _logger;

        public AdminsController(IAdminService adminService, ILogger<AdminsController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }
   //salamSALAM
        ///<summary>
        /// Yeni admin qeydiyyatı
        /// </summary>
        // [HttpPost("register")]
        // public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(new
        //         {
        //             StatusCode = StatusCodes.Status400BadRequest,
        //             Error = "Yanlış daxiletmə məlumatı!"
        //         });
        //     }
        //
        //     try
        //     {
        //         await _adminService.RegisterAdminAsync(registerDto);
        //         return StatusCode(StatusCodes.Status201Created, new
        //         {
        //             StatusCode = StatusCodes.Status201Created,
        //             Message = "İstifadəçi uğurla yaradıldı!"
        //         });
        //     }
        //     catch (GlobalAppException ex)
        //     {
        //         _logger.LogError(ex, "Admin qeydiyyatı zamanı xəta baş verdi!");
        //         return BadRequest(new
        //         {
        //             StatusCode = StatusCodes.Status400BadRequest,
        //             Error = ex.Message
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Admin qeydiyyatı zamanı gözlənilməz xəta baş verdi!");
        //         return StatusCode(StatusCodes.Status500InternalServerError, new
        //         {
        //             StatusCode = StatusCodes.Status500InternalServerError,
        //             Error = "Gözlənilməz xəta baş verdi. Zəhmət olmasa, yenidən cəhd edin!"
        //         });
        //     }
        // }

        /// <summary>
        /// Admin login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = "Yanlış daxiletmə məlumatı!"
                });
            }

            try
            {
                var result = await _adminService.LoginAdminAsync(loginDto);
                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,

                    Data = result
                });
            }
            catch (GlobalAppException ex)
            {
                _logger.LogError(ex, "Admin daxil olarkən xəta baş verdi!");
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Error = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin daxil olarkən gözlənilməz xəta baş verdi!");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Error = "Gözlənilməz xəta baş verdi. Zəhmət olmasa, yenidən cəhd edin!"
                });
            }
        }

        /// <summary>
        /// Admin istifadəçisini silmə
        /// </summary>
        //[HttpDelete("delete-all")]
        //public async Task<IActionResult> DeleteAllAdmins()
        //{
        //    try
        //    {
        //        await _adminService.DeleteAllAdminsAsync();
        //        return Ok(new
        //        {
        //            StatusCode = StatusCodes.Status200OK,
        //            Message = "Bütün adminlər uğurla silindi!"
        //        });
        //    }
        //    catch (GlobalAppException ex)
        //    {
        //        _logger.LogError(ex, "Bütün adminlər silinərkən xəta baş verdi!");
        //        return BadRequest(new
        //        {
        //            StatusCode = StatusCodes.Status400BadRequest,
        //            Error = ex.Message
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Bütün adminlər silinərkən gözlənilməz xəta baş verdi!");
        //        return StatusCode(StatusCodes.Status500InternalServerError, new
        //        {
        //            StatusCode = StatusCodes.Status500InternalServerError,
        //            Error = "Gözlənilməz xəta baş verdi. Zəhmət olmasa, yenidən cəhd edin!"
        //        });
        //    }
        //}

    }
}
