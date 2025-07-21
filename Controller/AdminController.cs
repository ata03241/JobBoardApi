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


    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto  dto)
    {
        var validrole = new[] { UserRoles.Admin, UserRoles.Employer, UserRoles.JobSeeker };
        if (!validrole.Contains(dto.Role))
        {
            return BadRequest("Invalid role specified.");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Role = dto.Role;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return Ok("User role updated successfully.");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var user = await _context.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Role = u.Role
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

    [HttpGet("jobs/pending")]
    public async Task<IActionResult> GetPendingJobs()
    {
        var pendingJobs = await _context.Jobs
            .Where(j => !j.IsApproved)
            .Select(j => new JobSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                CompanyName = j.CompanyName,
                Description = j.Description
            })
            .ToListAsync();

        if (pendingJobs == null || !pendingJobs.Any())
        {
            return NotFound("No pending jobs found.");
        }

        return Ok(pendingJobs);
    }

    [HttpPost("jobs/approve/{id}")]
    public async Task<IActionResult> ApproveJob(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return NotFound("Job not found.");
        }

        job.IsApproved = true;
        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();
        return Ok("Job approved successfully.");
    }

    [HttpDelete("jobs/{id}")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return NotFound("Job not found.");
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return Ok("Job deleted successfully.");
    }

}
