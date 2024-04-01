using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models;

public partial class UniversityContext : DbContext
{
    public UniversityContext()
    {
    }

    public UniversityContext(DbContextOptions<UniversityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server = A1SOFTECHPRO\\SQLEXPRESS; Database = University ; Trusted_Connection = true ; Encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Course__2AA84FD16C8F769F");

            entity.ToTable("Course");

            entity.HasIndex(e => e.DepartmentId, "IX_Course_departmentId");

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

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Logs__3214EC077A7CBCBC");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => new { e.ApplicationUserId, e.Id });

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.RefreshTokens).HasForeignKey(d => d.ApplicationUserId);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__4D11D63CFF68FF28");

            entity.ToTable("Student");

            entity.HasIndex(e => e.DepartmentId, "IX_Student_departmentId");

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
                        j.HasIndex(new[] { "CourseId" }, "IX_StudentInCourse_courseId");
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
