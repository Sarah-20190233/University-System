////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Threading.Tasks;
////using Application.Services;
////using AutoMapper;
////using Core.Models;
////using Infrastructure;
////using Infrastructure.Repositories;
////using Microsoft.AspNetCore.Routing;
////using Microsoft.EntityFrameworkCore;
////using Moq;
////using NUnit.Framework;


////namespace UniversityTest.Systems.Services
////{
////    [TestFixture]
////    public class TestStudentService
////    {
////        private DbContextOptions<ApplicationDbContext> _options;

////        [SetUp]
////        public void Setup()
////        {
////            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
////                .UseInMemoryDatabase(databaseName: "University")
////                .Options;
////        }

////        [Test]
////        public async Task GetAllStudents_ShouldReturnListOfStudentDtos()
////        {
////            // Arrange
////            var expectedStudents = new List<Student>
////            {
////                new Student {   StudentId = "1001",  Name = "John Doe",
////                    Gpa = 3.5,  Level = "Senior"
////                },
////                new Student {StudentId = "1002",
////                    Name = "Jane Smith",
////                    Gpa = 3.8,
////                    Level = "Junior" }

////            };

////            using (var context = new ApplicationDbContext(_options))
////            {
////                context.Students.AddRange(expectedStudents);
////                context.SaveChanges();
////            }

////            using (var context = new ApplicationDbContext(_options))
////            {
////                var mapperMock = new Mock<IMapper>(); // Assuming you have IMapper interface for mapping
////                var studentRepo = new StudentRepo(context, (IMapper)mapperMock);

////                var paginationParams = new PaginationParams(); // You can create a mock for paginationParams if needed
////                var studentService = new StudentService(studentRepo, mapperMock.Object);

////                // Act
////                var result = await studentService.GetAllStudents(paginationParams);

////                // Assert
////                Xunit.Assert.NotNull(result);
////                Xunit.Assert.Equal(expectedStudents.Count, result.Count);
////                for (int i = 0; i < expectedStudents.Count; i++)
////                {
////                    Xunit.Assert.Equal(expectedStudents[i].Name, result[i].Name);
////                }
////            }
////        }
////    }
////}


//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Application.Services;
//using AutoMapper;
//using Core.Models;
//using Infrastructure;
//using Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Moq;
//using NUnit.Framework;


//namespace UniversityTest.Systems.Services
//{
//    [TestFixture]
//    public class TestStudentService
//    {
//        private DbContextOptions<ApplicationDbContext> _options;

//        [SetUp]
//        public void Setup()
//        {
//            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: "University")
//                .Options;
//        }

//        [Test]
//        public async Task GetAllStudents_ShouldReturnListOfStudentDtos()
//        {
//            // Arrange
//            var expectedStudents = new List<Student>
//            {
//                new Student { StudentId = "1001", Name = "John Doe", Gpa = 3.5, Level = "Senior" },
//                new Student { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" }
//            };

//            using (var context = new ApplicationDbContext(_options))
//            {
//                context.Students.AddRange(expectedStudents);
//                context.SaveChanges();
//            }

//            using (var context = new ApplicationDbContext(_options))
//            {
//                var mapperMock = new Mock<IMapper>();
//                var studentRepo = new StudentRepo(context, mapperMock.Object);
//                var paginationParams = new PaginationParams();
//                var studentService = new StudentService(studentRepo, mapperMock.Object);

//                // Act
//                var result = await studentService.GetAllStudents(paginationParams);

//                // Assert
//                Xunit.Assert.NotNull(result);
//                Xunit.Assert.Equal(expectedStudents.Count, result.Count);
//                for (int i = 0; i < expectedStudents.Count; i++)
//                {
//                    Xunit.Assert.Equal(expectedStudents[i].Name, result[i].Name);
//                }
//            }
//        }
//    }
//}


using Xunit;
using Moq;
using Application.Services;
using Core.Models;
using Infrastructure.Repositories;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models.DTOs;
using Core.InterfacesOfRepo;

namespace UniversityTest.Systems.Services
{
    public class StudentServiceTests
    {
        [Fact]
        public async Task GetAllStudents_ShouldReturnMappedStudentDtos()
        {
            // Arrange
            var paginationParams = new PaginationParams(); // You can create a mock for paginationParams if needed

            // Mocking the repository
            var studentRepoMock = new Mock<IStudentRepo>();
            var expectedStudents = new List<Student>
            {
                new Student { Id = 1, Name = "Alice" },
                new Student { Id = 2, Name = "Bob" }
            };
            studentRepoMock.Setup(repo => repo.GetAll(paginationParams)).ReturnsAsync(expectedStudents);

            // Mocking the mapper
            var mapperMock = new Mock<IMapper>();
            var expectedStudentDtos = new List<StudentDto>
            {
                new StudentDto {StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior"},
                new StudentDto {StudentId = "1003", Name = "Jane Willy", Gpa = 3.5, Level = "Senior"}
            };
            mapperMock.Setup(mapper => mapper.Map<List<StudentDto>>(expectedStudents)).Returns(expectedStudentDtos);

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.GetAllStudents(paginationParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedStudentDtos.Count, result.Count);
            for (int i = 0; i < expectedStudentDtos.Count; i++)
            {
                Assert.Equal(expectedStudentDtos[i].Name, result[i].Name);
            }
        }


        [Fact]
        public async Task GetStudentCount_ShouldReturnCount()
        {
            // Arrange
            var expectedCount = 4; // Set the expected count of students

            var studentRepoMock = new Mock<IStudentRepo>();
            var mapperMock = new Mock<IMapper>();
            studentRepoMock.Setup(repo => repo.GetStudentCount()).ReturnsAsync(expectedCount);

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.GetStudentCount();

            // Assert
            Assert.Equal(expectedCount, result);
        }


        [Fact]
        public async Task SearchStudents_ShouldReturnMatchingStudents()
        {
            // Arrange
            var keyword = "Jane Smith"; // Set the search keyword
            var expectedStudents = new List<Student>
            {
                new Student { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" },
                new Student { StudentId = "1003", Name = "Jane Smith", Gpa = 3.7, Level = "Senior" }
            };

            var expectedStudentDtos = expectedStudents
                .Select(student => new StudentDto
                {
                    StudentId = student.StudentId,
                    Name = student.Name,
                    Gpa = student.Gpa,
                    Level = student.Level
                })
                .ToList();

            var studentRepoMock = new Mock<IStudentRepo>();
            var mapperMock = new Mock<IMapper>();

            studentRepoMock.Setup(repo => repo.SearchStudents(keyword)).ReturnsAsync(expectedStudents);
            mapperMock.Setup(m => m.Map<List<StudentDto>>(expectedStudents)).Returns(expectedStudentDtos);

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.SearchStudents(keyword);

            // Assert
            Assert.Equal(expectedStudentDtos.Count, result.Count);
      
        }



        [Fact]
        public async Task GetStudentById_ShouldReturnMappedStudentDto()
        {
            // Arrange
            var studentId = "1"; // Set the student ID
            var expectedStudentDto = new StudentDto { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" };

            var studentRepoMock = new Mock<IStudentRepo>();
            studentRepoMock.Setup(repo => repo.GetById(studentId)).ReturnsAsync(new Student { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior", DepartmentId ="1"});

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<StudentDto>(It.IsAny<Student>())).Returns(expectedStudentDto);

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.GetStudentById(studentId);

            // Assert
            Assert.Equal(expectedStudentDto.StudentId, result.StudentId);
            Assert.Equal(expectedStudentDto.Name, result.Name);
            Assert.Equal(expectedStudentDto.Gpa, result.Gpa);
            Assert.Equal(expectedStudentDto.Level, result.Level);

        }

        [Fact]
        public async Task AddStudent_ShouldReturnTrueWhenStudentAddedSuccessfully()
        {
            // Arrange
            var studentDto = new StudentDto { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" };

            var studentRepoMock = new Mock<IStudentRepo>();
            studentRepoMock.Setup(repo => repo.Add(It.IsAny<Student>())).ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Student>(studentDto)).Returns(new Student { StudentId = "1002", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" });

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.AddStudent(studentDto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturnTrueWhenStudentUpdatedSuccessfully()
        {
            // Arrange
            var studentId = "1"; // Assuming student ID exists
            var studentUpdatedDto = new StudentUpdatedDto { Gpa = 3.8, Level = "Junior" };
            var existingStudent = new Student { Gpa = 3 , Level = "Freshman" };

            var studentRepoMock = new Mock<IStudentRepo>();
            studentRepoMock.Setup(repo => repo.GetById(studentId)).ReturnsAsync(existingStudent);
            studentRepoMock.Setup(repo => repo.Update(It.IsAny<Student>())).ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map(studentUpdatedDto, existingStudent)).Verifiable();

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.UpdateStudent(studentId, studentUpdatedDto);

            // Assert
            Assert.True(result);
            mapperMock.Verify(mapper => mapper.Map(studentUpdatedDto, existingStudent), Times.Once);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnTrueWhenStudentDeletedSuccessfully()
        {
            // Arrange
            var studentId = "1"; // Assuming student ID exists

            var studentRepoMock = new Mock<IStudentRepo>();
            var mapperMock = new Mock<IMapper>();
            studentRepoMock.Setup(repo => repo.Delete(studentId)).ReturnsAsync(true);

            var studentService = new StudentService(studentRepoMock.Object, mapperMock.Object);

            // Act
            var result = await studentService.DeleteStudent(studentId);

            // Assert
            Assert.True(result);
        }




    }
}


