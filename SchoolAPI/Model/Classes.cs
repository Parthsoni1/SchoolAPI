using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SchoolAPI.Model
{
    public class Classes
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        public string Name { get; set; }

        [Required]
        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]

        public virtual Teacher? Teacher { get; set; }
    }
}
