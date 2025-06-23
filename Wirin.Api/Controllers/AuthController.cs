using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wirin.Domain.Models;
using Wirin.Api.Dtos.Requests;
using Wirin.Api.Dtos.Response;
using Wirin.Domain.Services;
using Wirin.Domain.Dtos.OCR;

namespace Wirin.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IConfiguration _conf;

    public AuthController(UserService userService, IConfiguration conf)
    {
        _userService = userService;
        _conf = conf;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest login)
    {
        var user = await _userService.GetByEmailAsync(login.Email);
        var userValidPassword = await _userService.CheckPasswordAsync(user, login.Password);

        if (user == null || !userValidPassword)
            return Unauthorized();

        var roles = user.Roles;
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userName", user.UserName),
            new Claim("fullName", user.FullName ?? ""),
            new Claim("phoneNumber", user.PhoneNumber ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtKey = _conf["AppConfig:Jwt:Key"];
        var jwtIssuer = _conf["AppConfig:Jwt:Issuer"];
        var jwtAudience = _conf["AppConfig:Jwt:Audience"];

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            return StatusCode(500, "Faltan configuraciones JWT en el entorno");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(5),
            signingCredentials: creds
        );

        return Ok(new LoginResponse
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo,
            userId = user.Id
        });
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var userExists = await _userService.GetByEmailAsync(request.Email);
        if (userExists != null)
            return BadRequest(new { message = "El correo ya esta registrado" });

        var userNameExists = await _userService.GetUserByUserNameAsync(request.UserName);
        if (userNameExists != null)
            return BadRequest(new { message = "El nombre de usuario ya esta registrado" });

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Roles = request.Roles
        };

        var result = await _userService.AddUserAsync(user, request.Password);

        if (!result)
        {
            return BadRequest(new { message = "Error al crear el usuario." });
        }

        return Ok(new { message = "Usuario registrado correctamente." });
    }
}
