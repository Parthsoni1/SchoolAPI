using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SchoolAPI.Model
{
    public class Student
    {

        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Role? Role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public int Id { get; set; }
        public string? Grade { get; set; }
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Classes? Classes { get; set; }
    }
    public enum Role
    {
        Student = 1,
        Teacher = 2,
        Principal = 3
    }
}
