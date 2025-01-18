using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using ChatGptProxyApi.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly IConfiguration _configuration;

  public AuthenticationController(UserManager<IdentityUser> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  [HttpPost("register")]
  public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto model)
  {
    var userExists = await _userManager.FindByNameAsync(model.Username);
    if (userExists != null)
      return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

    IdentityUser user = new IdentityUser { UserName = model.Username, Email = model.Email };
    var result = await _userManager.CreateAsync(user, model.Password);

    if (!result.Succeeded)
      return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });

    return Ok(new { Status = "Success", Message = "User created successfully!" });
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] UserLoginDto model)
  {
    var user = await _userManager.FindByNameAsync(model.Username);
    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
    {
      var token = GenerateJwtToken(user);

      return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
    }

    return Unauthorized(new { Status = "Error", Message = "User login failed. Incorrect username or password." });
  }

  private JwtSecurityToken GenerateJwtToken(IdentityUser user)
  {
    var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        expires: DateTime.Now.AddHours(3),
        claims: authClaims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return token;
  }
}
