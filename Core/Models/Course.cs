using System;
using System.Collections.Generic;

namespace Core.Models;

public partial class Course: BaseEntity
{
    //public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? CourseName { get; set; }

    public string? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
