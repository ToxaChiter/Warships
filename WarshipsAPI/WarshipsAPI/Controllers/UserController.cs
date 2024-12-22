using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WarshipsAPI.Logic.Interfaces;

namespace WarshipsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    //[HttpGet("profile")]
    //public async Task<IActionResult> GetProfile() { /* Получение данных профиля текущего пользователя */ }

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request.Email, request.Password);
        if (result is null)
        {
            return BadRequest("Reg error");
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request.Email, request.Password);
        if (result is null)
        {
            return BadRequest("Auth error");
        }

        return Ok(result);
    }
}
