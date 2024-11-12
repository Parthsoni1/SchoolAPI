using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data;
using SchoolAPI.Model;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        public readonly CrudDBContext _context;
        public ClassesController(CrudDBContext context)
        {
            _context = context;
        }

        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Classes>>> GetClasses()
        {
            return await _context.Classes.Include(c => c.Teacher).ToListAsync();
        }

        // GET: api/Classes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Classes>> GetClass(int id)
        {
            var classes = await _context.Classes.Include(c => c.Teacher).FirstOrDefaultAsync(c => c.Id == id);

            if (classes == null)
            {
                return NotFound();
            }

            return classes;
        }

        // POST: api/Classes
        [HttpPost]
        public async Task<ActionResult<Classes>> CreateClass(Classes newClass)
        {
            if (!_context.Teacher.Any(t => t.Id == newClass.TeacherId))
            {
                return BadRequest("Invalid TeacherId");
            }

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClass), new { id = newClass.Id }, newClass);
        }

        // PUT: api/Classes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, Classes updatedClass)
        {
            // Check if the Teacher exists
            if (!_context.Teacher.Any(t => t.Id == updatedClass.TeacherId))
            {
                return BadRequest("Invalid TeacherId");
            }

            // Retrieve the original entity
            var existingClass = await _context.Classes.FindAsync(id);
            if (existingClass == null)
            {
                return NotFound("Class not found.");
            }

            // Update the fields
            existingClass.Name = updatedClass.Name;
            existingClass.TeacherId = updatedClass.TeacherId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
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
        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var classes = await _context.Classes.FindAsync(id);
            if (classes == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(classes);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
