using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Tickets.Application.Services;
using Tickets.Domain.Entities;
using Tickets.Domain.Interfaces;
using Tickets.DTOs;

namespace Tickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly AuthService _authservice;

        public AccountController(AuthService authService)
        {
            _authservice = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            Usuario newUser = new Usuario()
            {
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                Tel = registerUserDto.Phone,
                Password = registerUserDto.Password
            };

            var result = await _authservice.RegisterUser(newUser);
            return Ok("Registro Exitoso");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authservice.Login(loginDto.Email, loginDto.Password, loginDto.RememberMe);
            
            return Ok(new { Token = result });
        }
    }
}
