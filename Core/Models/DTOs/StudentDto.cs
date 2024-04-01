
namespace Core.Models.DTOs
{
    public class StudentDto
    {
        public string StudentId { get; set; } = null!;

        public string? Name { get; set; }

        public double? Gpa { get; set; }

        public string? Level { get; set; }

        //public string? DepartmentId { get; set; }

        //public int? Id { get; set; }

        //public virtual Department? Department { get; set; }

       // public virtual ICollection<Course> Courses { get; set; } = new List<Course>();




    }

}
