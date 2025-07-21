using System;
using JobBoardApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobBoardApi.Controller;

[Route("api/[controller]")]
[ApiController]

public class JobController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobDto>>> GetJobs()
    {
        var jobs = await _context.Jobs
            .Where(j => j.IsApproved)
            .Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                PostedDate = j.PostedDate,
                CompanyName = j.CompanyName
            }).ToListAsync();

        
        if (jobs == null || !jobs.Any())
        {
            return NotFound();
        }
        return Ok(jobs);
    }

    [HttpGet("{id}")] // Get a specific job by ID
    public async Task<ActionResult<JobDto>> GetJOb(int id)
    {
        var job = await _context.Jobs.
            Where(j => j.Id == id && j.IsApproved)
            .Select(j => new JobDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                PostedDate = j.PostedDate,
                CompanyName = j.CompanyName
            })
            .FirstOrDefaultAsync();
        
        if (job == null)
        {
            return NotFound();
        }
        return Ok(job);
    }
    
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Employer)]
    [HttpPost]
    public async Task<ActionResult<JobDto>> CreateJOb(JobDto jobDto)
    {
        if (jobDto == null)
        {
            return BadRequest("Job cannot be null");
        }

        var job = new Job
        {
            Title = jobDto.Title,
            Description = jobDto.Description,
            PostedDate = jobDto.PostedDate,
            CompanyName = jobDto.CompanyName,
            IsApproved = false 
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var createdDtoJobb = new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            PostedDate = job.PostedDate,
            CompanyName = job.CompanyName
        };

        return CreatedAtAction(nameof(GetJOb), new { id = job.Id }, createdDtoJobb);
    }

    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Employer)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJob(int id, JobDto job)
    {
        if (id != job.Id)
        {
            return BadRequest("Job ID mismatch");
        }

        var existingJob = await _context.Jobs.FindAsync(id);
        if (existingJob == null)
        {
            return NotFound();
        }
        existingJob.Title = job.Title;
        existingJob.Description = job.Description;
        existingJob.CompanyName = job.CompanyName;
        existingJob.PostedDate = job.PostedDate;
        _context.Jobs.Update(existingJob);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Employer)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteJob(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();

        return NoContent(); // Returns 204 No Content
    }

}
