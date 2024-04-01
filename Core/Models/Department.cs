using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Department: BaseEntity
{
    //public string Id { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public string? DepartmentName { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
