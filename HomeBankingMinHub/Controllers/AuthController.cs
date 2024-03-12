using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientService _clientService;
        public AuthController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ClientLoginDTO client)
        {
            try
            {
                //encripto la password para chequear lo que me viene del login con lo que esta guardado en la base de datos
                String clientPasswordHashed = Encryptor.EncryptPassword(client.Password);

                Client user = _clientService.GetClientByEmail(client.Email);
                if (user == null || !String.Equals(user.Password, clientPasswordHashed))
                {
                    return Unauthorized();
                }

                var claims = new List<Claim>
                {
                    new Claim(user.Email.Contains("@itr.com") ? "Admin" : "Client", user.Email)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
