using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolAPI.Data;
using SchoolAPI.Model;

namespace SchoolAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        public readonly CrudDBContext Context;

        public TeacherController(CrudDBContext context)
        {
            Context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetStudent(string? search = null,
            string? sortBy = "Name",
            string sortOrder = "asc",
            int pageNumber = 1,
            int pageSize = 10)

        {
            // Get the IQueryable<Student> to apply filters, sort, and pagination
            IQueryable<Teacher> query = Context.Teacher;

            // Search by Name
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.UserName.Contains(search));
            }

            // Sorting
            //query = sortBy.ToLower() switch
            //{
            //    "age" => sortOrder == "asc" ? query.OrderBy(s => s.Age) : query.OrderByDescending(s => s.Age),
            //    _ => sortOrder == "asc" ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
            //};

            // Pagination
            var totalRecords = await query.CountAsync();
            var teacher = await query.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            // Adding pagination metadata (optional, you can include this if needed)
            var metadata = new
            {
                TotalCount = totalRecords,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)System.Math.Ceiling(totalRecords / (double)pageSize),
                HasNext = pageNumber < (int)System.Math.Ceiling(totalRecords / (double)pageSize),
                HasPrevious = pageNumber > 1,
                Student = teacher
            };

            // You can return metadata with Teachers or just students, for now returning students
            return Ok(metadata);

        }

        [HttpPost]
        public async Task<ActionResult> CreateTeacher(Teacher teacher)
        {
            Context.Add(teacher);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> getStudentById(int id)
        {
            var teacher = await Context.Students.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return Ok(teacher);
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult<Teacher>> deleteStudent(int id)
        {
            var teacher = await Context.Students.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            Context.Students.Remove(teacher);
            await Context.SaveChangesAsync();
            return NoContent();
        }

    }
}
