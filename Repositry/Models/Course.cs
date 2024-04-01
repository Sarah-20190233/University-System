using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string? CourseName { get; set; }

    public string? DepartmentId { get; set; }

    public int Id { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
