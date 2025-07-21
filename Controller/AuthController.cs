using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JobBoardApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobBoardApi.Controller;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existinguser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email);

        if (existinguser != null)
        {
            return BadRequest("Username or Email already exists.");
        }

        CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedAt = DateTime.UtcNow,
            Role = UserRoles.JobSeeker
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "User registered successfully. Awaiting approval." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        bool passwordValid = VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt);
        if (!passwordValid)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token
        });
    }

    private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

        var TokenDescription = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim("id", user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(TokenDescription);
        return tokenHandler.WriteToken(token);
    }
}
