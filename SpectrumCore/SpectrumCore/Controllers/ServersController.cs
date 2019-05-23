using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpectrumCore.Models;
using Microsoft.Extensions.Logging;

namespace SpectrumCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServersController : ControllerBase
    {
        private readonly ServersContext _context;
        private ILogger<Server> _logger;
        private const int MaxPlayers = 8;

        public ServersController(ServersContext context, ILogger<Server> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Servers
        [HttpGet]
        public IEnumerable<Server> GetServer()
        {
            return _context.Server;
        }

        // GET: api/Servers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var server = await _context.Server.FindAsync(id);

            if (server == null)
            {
                return NotFound();
            }

            return Ok(server);
        }

        // GET: api/Servers/5
        [HttpGet("match")]
        public IActionResult GetActiveMatch()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var server = _context.Server.Where(x => x.Players < MaxPlayers).FirstOrDefault();

            if (server == null)
            {
                return NotFound();
            }

            return Ok(server);
        }

        // PUT: api/Servers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServer([FromRoute] int id, [FromBody] Server server)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != server.Id)
            {
                return BadRequest();
            }

            server.LastCheckedIn = System.DateTime.Now;
            _context.Entry(server).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerExists(id))
                {
                    _context.Add(server);
                    await _context.SaveChangesAsync();
                    return Ok(new StatusMessage { Shutdown = false });
                }
                else
                {
                    throw;
                }
            }

            if(server.Players==0 && server.PlayedGames > 0)
            {
                return Ok(new StatusMessage { Shutdown = true });
            }
            else
            {
                return Ok(new StatusMessage { Shutdown = false });
            }
        }

        // POST: api/Servers
        [HttpPost]
        public async Task<IActionResult> PostServer([FromBody] Server server)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            server.LastCheckedIn = System.DateTime.Now;
            _context.Server.Add(server);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServer", new { id = server.Id }, server);
        }

        // DELETE: api/Servers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var server = await _context.Server.FindAsync(id);
            if (server == null)
            {
                return NotFound();
            }

            _context.Server.Remove(server);
            await _context.SaveChangesAsync();

            return Ok(server);
        }

        private bool ServerExists(int id)
        {
            return _context.Server.Any(e => e.Id == id);
        }
    }
}