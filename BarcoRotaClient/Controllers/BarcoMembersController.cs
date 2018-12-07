using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BarcoRota.Client.Models;
using BarcoRota.Models;

namespace BarcoRota.Client.Controllers
{
    [Produces("application/json")]
    [Route("api/BarcoMembers")]
    public class BarcoMembersController : Controller
    {
        private readonly BarcoContext _context;

        public BarcoMembersController(BarcoContext context)
        {
            _context = context;
        }

        // GET: api/BarcoMembers
        [HttpGet]
        public IEnumerable<BarcoMember> GetBarcoMembers()
        {
            return _context.BarcoMembers;
        }

        // GET: api/BarcoMembers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBarcoMember([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var barcoMember = await _context.BarcoMembers.SingleOrDefaultAsync(m => m.Id == id);

            if (barcoMember == null)
            {
                return NotFound();
            }

            return Ok(barcoMember);
        }

        // PUT: api/BarcoMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBarcoMember([FromRoute] int id, [FromBody] BarcoMember barcoMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != barcoMember.Id)
            {
                return BadRequest();
            }

            _context.Entry(barcoMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BarcoMemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BarcoMembers
        [HttpPost]
        public async Task<IActionResult> PostBarcoMember([FromBody] BarcoMember barcoMember)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.BarcoMembers.Add(barcoMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBarcoMember", new { id = barcoMember.Id }, barcoMember);
        }

        // DELETE: api/BarcoMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarcoMember([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var barcoMember = await _context.BarcoMembers.SingleOrDefaultAsync(m => m.Id == id);
            if (barcoMember == null)
            {
                return NotFound();
            }

            _context.BarcoMembers.Remove(barcoMember);
            await _context.SaveChangesAsync();

            return Ok(barcoMember);
        }

        private bool BarcoMemberExists(int id)
        {
            return _context.BarcoMembers.Any(e => e.Id == id);
        }
    }
}