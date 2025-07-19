using System;
using JobBoardApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoardApi.Controller;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController : ControllerBase
{
    private readonly AppDbContext _context;

    public ApplicationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Application>>> GetApplications()
    {
        var applications = await _context.Applications
            .Include(a => a.Job) // Include Job details
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                ApplicantName = a.ApplicantName,
                Email = a.Email,
                ResumeText = a.ResumeText,
                ApplicationDate = a.ApplicationDate,
                JobId = a.JobId,
                Job = new JobSummaryDto
                {
                    Id = a.Job.Id,
                    Title = a.Job.Title,
                    CompanyName = a.Job.CompanyName
                }
            })
            .ToListAsync();
        if (applications == null || !applications.Any())
        {
            return NotFound();
        }
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Application>> GetApplication(int id)
    {
        var application = await _context.Applications
            .Where(a => a.Id == id)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                ApplicantName = a.ApplicantName,
                Email = a.Email,
                ResumeText = a.ResumeText,
                ApplicationDate = a.ApplicationDate,
                JobId = a.JobId,
                Job = new JobSummaryDto
                {
                    Id = a.Job.Id,
                    Title = a.Job.Title,
                    CompanyName = a.Job.CompanyName
                }
            })
            .FirstOrDefaultAsync();
        if (application == null)
        {
            return NotFound();
        }
        return Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<Application>> CreateApplication(ApplicationDto dto)
    {
        var application = await _context.Jobs.FindAsync(dto.JobId);
        if (application == null)
        {
            return BadRequest("Job not found");
        }
        var newApplication = new Application
        {
            ApplicantName = dto.ApplicantName,
            Email = dto.Email,
            ResumeText = dto.ResumeText,
            ApplicationDate = DateTime.UtcNow,
            JobId = dto.JobId
        };
        _context.Applications.Add(newApplication);
        await _context.SaveChangesAsync();


        //fetch and create a DTO for the new application
        var applicationDto = _context.Applications
            .Where(a => a.Id == newApplication.Id)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                ApplicantName = a.ApplicantName,
                Email = a.Email,
                ResumeText = a.ResumeText,
                ApplicationDate = a.ApplicationDate,
                JobId = a.JobId,
                Job = new JobSummaryDto
                {
                    Id = a.Job.Id,
                    Title = a.Job.Title,
                    CompanyName = a.Job.CompanyName
                }
            })
            .FirstOrDefaultAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = newApplication.Id }, await applicationDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var application = await _context.Applications.FindAsync(id);
        if (application == null)
        {
            return NotFound();
        }

        _context.Applications.Remove(application);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
