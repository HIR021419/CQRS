using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;
using UserService.Models;
using UserService.Services;
using System.Text.Json;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserServiceContext _context;
        private readonly IntegrationEventSenderService _integrationEventSenderService;

        public UserController(UserServiceContext context, IntegrationEventSenderService integrationEventSenderService)
        {
            _context = context;
            _integrationEventSenderService = integrationEventSenderService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(AddUserDto userDtoDto)
        {
            // not dispo in inMemory storage
            //using var transaction = await _context.Database.BeginTransactionAsync();

            User user = new(userDtoDto.Name, userDtoDto.Email);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var integrationEventData = JsonSerializer.Serialize(new
            {
                id = user.Id,
                name = user.Name,
                version = user.Version
            });

            _context.IntegrationEventOutbox.Add(new IntegrationEvent
            {
                Event = "user.add",
                Data = integrationEventData
            });

            await _context.SaveChangesAsync();
            //await transaction.CommitAsync();

            _integrationEventSenderService.StartPublishingOutstandingIntegrationEvents();

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Name = updatedUser.Name;
            existingUser.Version += 1;

            using var transaction = await _context.Database.BeginTransactionAsync();

            await _context.SaveChangesAsync();

            var integrationEventData = JsonSerializer.Serialize(new
            {
                id = existingUser.Id,
                name = existingUser.Name,
                version = existingUser.Version
            });

            _context.IntegrationEventOutbox.Add(new IntegrationEvent
            {
                Event = "user.update",
                Data = integrationEventData
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _integrationEventSenderService.StartPublishingOutstandingIntegrationEvents();

            return Ok(existingUser);
        }
    }
}
