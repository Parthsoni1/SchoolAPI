using System.ComponentModel.DataAnnotations;


namespace SchoolAPI.Model
{
    public class Teacher
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
        public string Subject { get; set; }
        public string Department { get; set; }
    }
}
