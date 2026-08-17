using CricArena.Business.DTOs.Invitation;
using CricArena.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CricArena.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpGet("club/{clubId:guid}/players/search")]
        public async Task<ActionResult<List<InvitationPlayerSearchResponse>>> SearchPlayersByPhoneNumber(
            Guid clubId,
            [FromQuery] string phoneNumber)
        {
            var players = await _invitationService.SearchPlayersByPhoneNumberAsync(clubId, phoneNumber);
            return Ok(players);
        }

        [HttpPost]
        public async Task<ActionResult<InvitationResponse>> CreateInvitation(CreateInvitationRequest request)
        {
            var invitation = await _invitationService.CreateAsync(request);
            return CreatedAtAction(nameof(GetInvitationById), new { invitationId = invitation.Id }, invitation);
        }

        [HttpGet("club/{clubId:guid}")]
        public async Task<ActionResult<List<InvitationResponse>>> GetClubInvitations(Guid clubId)
        {
            var invitations = await _invitationService.GetClubInvitationsAsync(clubId);
            return Ok(invitations);
        }

        [HttpGet("mine")]
        public async Task<ActionResult<List<InvitationResponse>>> GetMyInvitations()
        {
            var invitations = await _invitationService.GetMyInvitationsAsync();
            return Ok(invitations);
        }

        [HttpGet("{invitationId:guid}")]
        public async Task<ActionResult<InvitationResponse>> GetInvitationById(Guid invitationId)
        {
            var invitation = await _invitationService.GetByIdAsync(invitationId);
            return Ok(invitation);
        }

        [HttpPost("{invitationId:guid}/accept")]
        public async Task<IActionResult> AcceptInvitation(Guid invitationId)
        {
            await _invitationService.AcceptAsync(invitationId);
            return NoContent();
        }

        [HttpPost("{invitationId:guid}/reject")]
        public async Task<IActionResult> RejectInvitation(Guid invitationId)
        {
            await _invitationService.RejectAsync(invitationId);
            return NoContent();
        }

        [HttpPost("{invitationId:guid}/cancel")]
        public async Task<IActionResult> CancelInvitation(Guid invitationId)
        {
            await _invitationService.CancelAsync(invitationId);
            return NoContent();
        }
    }
}
