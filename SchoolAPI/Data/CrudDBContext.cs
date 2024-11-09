using Microsoft.EntityFrameworkCore;
using SchoolAPI.Model;
namespace SchoolAPI.Data
{
    public class CrudDBContext: DbContext
    {
        public CrudDBContext(DbContextOptions<CrudDBContext> options) : base(options)
        {

        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
    }
}
