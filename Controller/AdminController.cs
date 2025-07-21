using System;
using Microsoft.AspNetCore.Mvc;
using JobBoardApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace JobBoardApi.Controller;

[ApiController]
[Route("admin")]
[Authorize(Roles = UserRoles.Admin)]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // Additional methods for admin functionalities can be added here
    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var pendinguser = await _context.Users
            .Where(u => !u.IsApproved)
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Role = u.Role
            })
            .ToListAsync();

        if (pendinguser == null || !pendinguser.Any())
        {
            return NotFound("No pending users found.");
        }

        return Ok(pendinguser);
    }

    [HttpPost("approve/{id}")]
    public async Task<IActionResult> ApproveUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.IsApproved = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Ok("User approved successfully.");
    }

    [HttpPost("reject/{id}")]
    public async Task<IActionResult> RejectUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok("User rejected and removed successfully.");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var user = await _context.Users
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Role = u.Role,
                IsApproved = u.IsApproved
            })
            .ToListAsync();

        if (user == null || !user.Any())
        {
            return NotFound("No users found.");
        }

        return Ok(user);
    }

    [HttpDelete("user/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok("User deleted successfully.");
    }
}
