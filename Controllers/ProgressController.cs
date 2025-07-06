using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLMS.DTOs;
using MiniLMS.Services;
using System.Security.Claims;

namespace MiniLMS.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;
    private readonly IUserService _userService;

    public ProgressController(IProgressService progressService, IUserService userService)
    {
        _progressService = progressService;
        _userService = userService;
    }

    [HttpGet("my-progress")]
    public async Task<ActionResult<IEnumerable<AssignmentProgressDto>>> GetMyProgress()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var progresses = await _progressService.GetUserProgressesAsync(userId);
        return Ok(progresses);
    }

    [HttpGet("subordinates")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<AssignmentProgressDto>>> GetSubordinateProgress()
    {
        var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(managerId))
            return Unauthorized();

        var progresses = await _progressService.GetSubordinateProgressesAsync(managerId);
        return Ok(progresses);
    }

    [HttpGet("assignment/{assignmentId}/report")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<ActionResult<IEnumerable<AssignmentProgressDto>>> GetAssignmentProgressReport(int assignmentId)
    {
        var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(managerId))
            return Unauthorized();

        var progresses = await _progressService.GetAssignmentProgressReportAsync(assignmentId, managerId);
        return Ok(progresses);
    }

    [HttpGet("{progressId}")]
    public async Task<ActionResult<AssignmentProgressDto>> GetProgressDetails(int progressId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var progress = await _progressService.GetProgressDetailsAsync(progressId, userId);
        if (progress == null)
            return NotFound();

        // Check if user can access this progress
        if (progress.UserId != userId)
        {
            var canView = await _userService.CanUserViewProgressAsync(userId, progress.UserId);
            if (!canView)
                return Forbid();
        }

        return Ok(progress);
    }
}
