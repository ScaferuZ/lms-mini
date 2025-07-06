using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.DTOs;
using MiniLMS.Services;
using System.Security.Claims;

namespace MiniLMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;

    public AssignmentsController(IAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetActiveAssignments()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var assignments = await _assignmentService.GetActiveAssignmentsAsync(userId);
        return Ok(assignments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var assignment = await _assignmentService.GetAssignmentDetailsAsync(id, userId);
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(AssignmentDto assignmentDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var createdAssignment = await _assignmentService.CreateAssignmentAsync(assignmentDto, userId);
            return CreatedAtAction(nameof(GetAssignment), new { id = createdAssignment.Id }, createdAssignment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult> StartAssignment(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var canAccess = await _assignmentService.CanUserAccessAssignmentAsync(id, userId);
        if (!canAccess)
            return Forbid();

        var started = await _assignmentService.StartAssignmentAsync(id, userId);
        if (!started)
            return Conflict("Assignment already started");

        return Ok();
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult<AssignmentProgressDto>> SubmitQuiz(int id, SubmitQuizDto submitDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        if (submitDto.AssignmentId != id)
            return BadRequest("Assignment ID mismatch");

        try
        {
            var progress = await _assignmentService.SubmitQuizAsync(submitDto, userId);
            return Ok(progress);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}/status")]
    public async Task<ActionResult<object>> GetAssignmentStatus(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var canAccess = await _assignmentService.CanUserAccessAssignmentAsync(id, userId);
        if (!canAccess)
            return Forbid();

        var completed = await _assignmentService.HasUserCompletedAssignmentAsync(id, userId);
        
        return Ok(new { 
            CanAccess = canAccess, 
            Completed = completed 
        });
    }
}
