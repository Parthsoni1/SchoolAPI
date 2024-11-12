using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data;
using SchoolAPI.Model;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public readonly CrudDBContext Context;

        public StudentController(CrudDBContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult> GetStudent(
    string? search = null,
    string? sortBy = "UserName",
    string sortOrder = "asc",
    int pageNumber = 1,
    int pageSize = 10)
        {
            // Start with the base query, including Class and Teacher relationships
            IQueryable<Student> query = Context.Students
                .Include(s => s.Classes)
                .ThenInclude(c => c.Teacher);

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.UserName.Contains(search));
            }

            // Sort by specific properties, allowing only valid properties
            var validSortByFields = new List<string> { "UserName", "Email", "Grade", "Id" };
            if (!validSortByFields.Contains(sortBy))
            {
                return BadRequest($"Invalid sort field: {sortBy}. Valid fields are: {string.Join(", ", validSortByFields)}");
            }

            // Sorting logic with dynamic sorting
            query = sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(s => EF.Property<object>(s, sortBy))
                : query.OrderBy(s => EF.Property<object>(s, sortBy));

            // Pagination
            var totalRecords = await query.CountAsync();
            var students = await query.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .Select(s => new
                                      {
                                          s.Id,
                                          s.UserName,
                                          s.Email,
                                          s.Phone,
                                          s.Address,
                                          Role = s.Role.ToString(),
                                          s.Password,
                                          s.Grade,
                                          ClassName = s.Classes != null ? s.Classes.Name : null,
                                          TeacherName = s.Classes.Teacher != null ? s.Classes.Teacher.UserName : null
                                      })
                                      .ToListAsync();

            // Pagination metadata
            var metadata = new
            {
                TotalCount = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNext = pageNumber < (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasPrevious = pageNumber > 1,
                Students = students
            };

            return Ok(metadata);
        }


        [HttpPost]
        public async Task<ActionResult<Student>> createStudent(Student student)
        {
            Context.Add(student);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id}/role")]
        public async Task<ActionResult<Student>> UpdateStudentRole(int id, [FromBody] Role newRole)
        {
            var student = await Context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.Role = newRole;
            // Mark only the Role property as modified
            Context.Entry(student).Property(s => s.Role).IsModified = true;
            await Context.SaveChangesAsync();
            return Ok(student);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> getStudentById(int id)
        {
            var student = await Context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<Student>> deleteStudent(int id)
        {
            var student = await Context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            Context.Students.Remove(student);
            await Context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            if (id != updatedStudent.Id)
            {
                return BadRequest("Student ID mismatch.");
            }

            var existingStudent = await Context.Students.FindAsync(id);
            if (existingStudent == null)
            {
                return NotFound("Student not found.");
            }

            // Update the student's properties
            existingStudent.UserName = updatedStudent.UserName;
            existingStudent.Email = updatedStudent.Email;
            existingStudent.Phone = updatedStudent.Phone;
            existingStudent.Address = updatedStudent.Address;
            existingStudent.Role = updatedStudent.Role;
            existingStudent.Password = updatedStudent.Password;
            existingStudent.Grade = updatedStudent.Grade;
            existingStudent.ClassId = updatedStudent.ClassId;

            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound("Student not found.");
                }
                throw;
            }

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return Context.Students.Any(e => e.Id == id);
        }

    }
}
