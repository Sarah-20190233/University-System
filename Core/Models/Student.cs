using System;
using System.Collections.Generic;


namespace Core.Models;

public partial class Student: BaseEntity
{
    //public string Id { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? Name { get; set; }

    public double? Gpa { get; set; }

    public string? Level { get; set; }

    public string? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
