using CricArena.Business.DTOs.Player;
using CricArena.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CricArena.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        /// <summary>
        /// Creates a new player.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PlayerResponse>> CreatePlayer(CreatePlayerRequest request)
        {
            var player = await _playerService.CreatePlayerAsync(request);

            return CreatedAtAction(
                nameof(GetPlayerById),
                new { id = player.Id },
                player);
        }

        /// <summary>
        /// Returns all players.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PlayerResponse>>> GetAllPlayers()
        {
            var players = await _playerService.GetAllPlayersAsync();

            return Ok(players);
        }

        /// <summary>
        /// Returns a player by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlayerResponse>> GetPlayerById(Guid id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);

            return Ok(player);
        }

        /// <summary>
        /// Updates an existing player.
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePlayer(
            Guid id,
            UpdatePlayerRequest request)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);

            await _playerService.UpdatePlayerAsync(id, request);

            return NoContent();
        }

        /// <summary>
        /// Deletes a player.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePlayer(Guid id)
        {
            var player = await _playerService.GetPlayerByIdAsync(id);

            await _playerService.DeletePlayerAsync(id);

            return NoContent();
        }


    }
}
