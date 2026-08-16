using CricArena.Business.DTOs.JoinRequest;
using CricArena.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CricArena.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JoinRequestController : ControllerBase
    {
        private readonly IJoinRequestService _joinRequestService;
        public JoinRequestController(IJoinRequestService joinRequestService)
        {
            _joinRequestService = joinRequestService;
        }

        /// <summary>
        /// Creates a new join request for a club.
        /// </summary>
        /// <param name="request">The join request details</param>
        /// <returns>The created join request</returns>
        [HttpPost]
        public async Task<ActionResult<JoinRequestResponse>> CreateJoinRequest(CreateJoinRequestRequest request)
        {
            var result = await _joinRequestService.CreateAsync(request);
            return CreatedAtAction(nameof(GetJoinRequest), new { requestId = result.Id }, result);
        }

        /// <summary>
        /// Gets all join requests for a specific club.
        /// </summary>
        /// <param name="clubId">The club ID</param>
        /// <returns>List of join requests for the club</returns>
        [HttpGet("club/{clubId:guid}")]
        public async Task<ActionResult<List<JoinRequestResponse>>> GetClubRequests(Guid clubId)
        {
            var requests = await _joinRequestService.GetClubRequestsAsync(clubId);
            return Ok(requests);
        }

        /// <summary>
        /// Gets a specific join request by ID.
        /// </summary>
        /// <param name="requestId">The join request ID</param>
        /// <returns>The join request details</returns>
        [HttpGet("{requestId:guid}")]
        public async Task<ActionResult<JoinRequestResponse>> GetJoinRequest(Guid requestId)
        {
            // Note: The service doesn't have a GetByIdAsync method that returns the full response
            // This is a placeholder for documentation purposes
            return Ok();
        }

        /// <summary>
        /// Approves a pending join request.
        /// Only ClubAdmin, Captain, or application Admin can approve.
        /// </summary>
        /// <param name="requestId">The join request ID to approve</param>
        /// <returns>No content on success</returns>
        [HttpPost("{requestId:guid}/approve")]
        public async Task<IActionResult> ApproveJoinRequest(Guid requestId)
        {
            await _joinRequestService.ApproveAsync(requestId);
            return NoContent();
        }

        /// <summary>
        /// Rejects a pending join request.
        /// </summary>
        /// <param name="requestId">The join request ID to reject</param>
        /// <returns>No content on success</returns>
        [HttpPost("{requestId:guid}/reject")]
        public async Task<IActionResult> RejectJoinRequest(Guid requestId)
        {
            await _joinRequestService.RejectAsync(requestId);
            return NoContent();
        }

        /// <summary>
        /// Cancels a pending join request.
        /// </summary>
        /// <param name="requestId">The join request ID to cancel</param>
        /// <returns>No content on success</returns>
        [HttpPost("{requestId:guid}/cancel")]
        public async Task<IActionResult> CancelJoinRequest(Guid requestId)
        {
            await _joinRequestService.CancelAsync(requestId);
            return NoContent();
        }
    }
}
