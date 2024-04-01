using Core.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
namespace Infrastructure;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    //public virtual DbSet<Logs> RequestResponseLogs { get; set; }

    public virtual DbSet<Logs> Logs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__2AA84FD16C8F769F");

            entity.ToTable("Course");

            entity.Property(e => e.CourseId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("courseId");
            entity.Property(e => e.CourseName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("courseName");
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("departmentId");

            entity.HasOne(d => d.Department).WithMany(p => p.Courses)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Course__departme__75A278F5");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__F9B8346D13640382");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("departmentId");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("departmentName");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__4D11D63CFF68FF28");

            entity.ToTable("Student");

            entity.Property(e => e.StudentId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("studentId");
            entity.Property(e => e.DepartmentId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("departmentId");
            entity.Property(e => e.Gpa).HasColumnName("GPA");
            entity.Property(e => e.Level)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Students)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Student__departm__76969D2E");

            entity.HasMany(d => d.Courses).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentInCourse",
                    r => r.HasOne<Course>().WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentIn__cours__778AC167"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentIn__stude__787EE5A0"),
                    j =>
                    {
                        j.HasKey("StudentId", "CourseId").HasName("PK__StudentI__4FBB52C1B3C9135A");
                        j.ToTable("StudentInCourse");
                        j.IndexerProperty<string>("StudentId")
                            .HasMaxLength(15)
                            .IsUnicode(false)
                            .HasColumnName("studentId");
                        j.IndexerProperty<string>("CourseId")
                            .HasMaxLength(15)
                            .IsUnicode(false)
                            .HasColumnName("courseId");
                    });
        });

        //modelBuilder.Entity<Logs>(entity =>
        //{
        //    entity.HasKey(e => e.Id).HasName("PK_RequestResponseLogs");

        //    entity.ToTable("RequestResponseLogs");

        //    entity.Property(e => e.Id)
        //        .HasColumnName("Id")
        //        .ValueGeneratedOnAdd()
        //        .HasColumnType("int")
        //        .IsRequired();

        //    entity.Property(e => e.Timestamp)
        //        .HasColumnName("Timestamp")
        //        .HasColumnType("datetime")
        //        .IsRequired();

        //    entity.Property(e => e.Method)
        //        .HasColumnName("Method")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.Path)
        //        .HasColumnName("Path")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.QueryString)
        //        .HasColumnName("QueryString")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.Headers)
        //        .HasColumnName("Headers")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.RequestBody)
        //        .HasColumnName("RequestBody")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.StatusCode)
        //        .HasColumnName("StatusCode")
        //        .HasColumnType("int")
        //        .IsRequired();

        //    entity.Property(e => e.ContentType)
        //        .HasColumnName("ContentType")
        //        .HasColumnType("nvarchar(max)");

        //    entity.Property(e => e.ResponseBody)
        //        .HasColumnName("ResponseBody")
        //        .HasColumnType("nvarchar(max)");

        //    entity.HasIndex(e => e.Id).IsUnique();

        //    entity.HasIndex(e => e.Timestamp);
        //});





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
