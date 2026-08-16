using CricArena.Business.DTOs.Club;
using CricArena.Business.Services.Interfaces;
using CricArena.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CricArena.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;
        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        /// <summary>
        /// Creates a new club.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ClubResponse>> CreateClub(CreateClubRequest request)
        {
            var club = await _clubService.CreateClubAsync(request);
            return CreatedAtAction(nameof(GetClubById), new { id = club.Id }, club);
        }

        /// <summary>
        /// Gets all clubs.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ClubResponse>>> GetAllClubs()
        {
            var clubs = await _clubService.GetAllClubsAsync();
            return Ok(clubs);
        }

        /// <summary>
        ///     Gets a specific club by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClubResponse>> GetClubById(Guid id)
        {
            var club = await _clubService.GetClubByIdAsync(id);
            return Ok(club);
        }

        /// <summary>
        ///     Deletes a specific club by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteClub(Guid id)
        {
            await _clubService.DeleteClubAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets all members of a specific club.
        /// </summary>
        /// <param name="clubId"></param>
        /// <returns></returns>
        [HttpGet("{clubId:guid}/members")]
        public async Task<ActionResult<List<ClubMemberResponse>>> GetMembers(Guid clubId)
        {
            var members = await _clubService.GetMembersAsync(clubId);
            return Ok(members);
        }

        /// <summary>
        /// Updates the role of a specific member in a club.
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="playerId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{clubId:guid}/members/{playerId:guid}/role")]
        public async Task<IActionResult> UpdateMemberRole(Guid clubId, Guid playerId, UpdateMemberRoleRequest request)
        {
            await _clubService.UpdateMemberRoleAsync(clubId, playerId, request);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateClub(Guid id, UpdateClubRequest request)
        {
            await _clubService.UpdateClubAsync(id, request);
            return NoContent();
        }
    }
}
