using System;
using System.Collections.Generic;

namespace Infrastructure.Models;

public partial class Department
{
    public string DepartmentId { get; set; } = null!;

    public string? DepartmentName { get; set; }

    public int Id { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
