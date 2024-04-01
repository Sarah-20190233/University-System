using Application.Services;
using AutoMapper;
using Core.InterfacesOfServices;
using Core.Models;
using Core.Models.DTOs;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using UniversityTest.MockData;
using UnversityManagement.Controllers;


namespace UniversityTest.Systems.Controllers
{
    public class TestStudentController
    {

        private readonly IStudentService _studentService;


     
       
        [Fact]
        public async Task GetStudents_ShouldReturn200Status()
        {
            /////Arrange
            ///
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();


            var paginationParams = new PaginationParams(); // Create a mock PaginationParams object if needed
            studentService.Setup(_ => _.GetAllStudents(paginationParams))
                .ReturnsAsync(StudentMockData.GetStudents()); // Adjust the setup to accept paginationParams

            //var sut = new StudentsController(studentService.Object);    
            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);



            // Act
            var actionResult = await sut.GetStudents(paginationParams);
            var result = actionResult.Result as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

        }

        [Fact]
        public async Task GetStudents_ShouldReturn204Status_WhenStudentRetrievalFails()
        {
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            // Arrange
         
            var paginationParams = new PaginationParams(); // Create a mock PaginationParams object if needed
            studentService.Setup(_ => _.GetAllStudents(paginationParams))
                .ReturnsAsync(StudentMockData.GetEmptyStudents()); // Adjust the setup to accept paginationParams

            // var sut = new StudentsController(studentService.Object);
            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.GetStudents(paginationParams);
            var result = actionResult.Result as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(204);
        }


   
        [Fact]
        public async Task GetStudentCount_ShouldReturn200Status()
        {
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            // Arrange
          
            var expectedCount = 4; // Set expected student count
            studentService.Setup(_ => _.GetStudentCount()).ReturnsAsync(expectedCount);
            // var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.GetStudentCount();
            var result = actionResult.Result as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be(expectedCount);
        }


        [Fact]
        public async Task GetStudentCountNoContent_ShouldReturn204Status_WhenStudentCountIsZero()
        {
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            // Arrange
           
            var expectedCount = 0; // Set expected student count to zero
            studentService.Setup(_ => _.GetStudentCount()).ReturnsAsync(expectedCount);
            //var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.GetStudentCount();

            // Assert
            actionResult.Result.Should().BeOfType<NoContentResult>();
            var result = actionResult.Result as NoContentResult;
            result.StatusCode.Should().Be(204);
        }

        

        [Fact]
        public async Task SearchStudents_ShouldReturn200Status()
        {
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            // Arrange
           

            // Create a list of sample students
            var expectedStudents = new List<Student>
    {
        new Student { StudentId = "1", Name = "John Doe", Gpa = 3.5, Level = "Junior" },
        new Student { StudentId = "2", Name = "Alice Smith", Gpa = 3.7, Level = "Senior" }
    };

            // Map the list of sample students to a list of StudentDto objects
            var expectedStudentDtos = expectedStudents
                .Select(student => new StudentDto
                {
                    StudentId = student.StudentId,
                    Name = student.Name,
                    Gpa = student.Gpa,
                    Level = student.Level
                })
                .ToList();

            // Set up the mock to return the list of StudentDto objects
            studentService.Setup(_ => _.SearchStudents(It.IsAny<string>())).ReturnsAsync(expectedStudentDtos);

            studentService.Setup(_ => _.SearchStudents(It.IsAny<string>())).ReturnsAsync(expectedStudentDtos);


            //  var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.SearchStudents("Arwa");
            var result = actionResult.Result as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeOfType<List<StudentDto>>(); // Adjust type if necessary
        }

        [Fact]
        public async Task SearchStudents_ShouldReturn404Status_WhenNoStudentsFound()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();    
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            var expectedStudents = new List<Student>(); // Empty list to simulate no students found

            // Map the list of sample students to a list of StudentDto objects
            var expectedStudentDtos = expectedStudents
                .Select(student => new StudentDto
                {
                 
                })
                .ToList();

            studentService.Setup(_ => _.SearchStudents(It.IsAny<string>())).ReturnsAsync(expectedStudentDtos);
            //var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.SearchStudents("Arwa");

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundResult>();
            var result = actionResult.Result as NotFoundResult;
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturn200Status()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            var expectedStudentId = "testStudentId"; // Set expected student ID
            var expectedStudent = new StudentDto(); // Create a mock student object if needed
            studentService.Setup(_ => _.GetStudentById(expectedStudentId)).ReturnsAsync(expectedStudent);
            // var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.Get(expectedStudentId);
            var result = actionResult as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeOfType<StudentDto>(); // Adjust type if necessary
        }

        [Fact]
        public async Task GetStudentById_ShouldReturn404Status_WhenStudentNotFound()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            var nonExistentStudentId = "nonExistentId";
            studentService.Setup(_ => _.GetStudentById(nonExistentStudentId)).ReturnsAsync((StudentDto)null); // Simulate no student found
            //var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);

            // Act
            var actionResult = await sut.Get(nonExistentStudentId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
            var result = actionResult as NotFoundResult;
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PostStudent_ShouldReturn200Status_WhenAddingStudentSucceeds()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.AddStudent(It.IsAny<StudentDto>())).ReturnsAsync(true);
            //var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentDto = new StudentDto(); // Create a mock student DTO object for testing

            // Act
            var actionResult = await sut.Post(studentDto);
            var result = actionResult as OkResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostStudent_ShouldReturn500Status_WhenAddingStudentFails()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.AddStudent(It.IsAny<StudentDto>())).ReturnsAsync(false);
            //   var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentDto = new StudentDto(); // Create a mock student DTO object for testing

            // Act
            var actionResult = await sut.Post(studentDto);
            var result = actionResult as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
            result.Value.Should().Be("Failed to add student.");
        }


        [Fact]
        public async Task PutStudent_ShouldReturn200Status_WhenUpdatingStudentSucceeds()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.UpdateStudent(It.IsAny<string>(), It.IsAny<StudentUpdatedDto>())).ReturnsAsync(true);
            //var sut = new StudentsController(studentService.Object);
            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentId = "testStudentId";
            var studentUpdatedDto = new StudentUpdatedDto(); // Create a mock student updated DTO object for testing

            // Act
            var actionResult = await sut.Put(studentId, studentUpdatedDto);
            var result = actionResult as OkResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PutStudent_ShouldReturn404Status_WhenUpdatingStudentFails()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.UpdateStudent(It.IsAny<string>(), It.IsAny<StudentUpdatedDto>())).ReturnsAsync(false);
            //var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentId = "testStudentId";
            var studentUpdatedDto = new StudentUpdatedDto(); // Create a mock student updated DTO object for testing

            // Act
            var actionResult = await sut.Put(studentId, studentUpdatedDto);
            var result = actionResult as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }


        [Fact]
        public async Task DeleteStudent_ShouldReturn200Status_WhenDeletingStudentSucceeds()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.DeleteStudent(It.IsAny<string>())).ReturnsAsync(true);
            //  var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentId = "testStudentId";

            // Act
            var actionResult = await sut.Delete(studentId);
            var result = actionResult as OkResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturn404Status_WhenDeletingStudentFails()
        {
            // Arrange
            var studentService = new Mock<IStudentService>();
            var dbContext = new Mock<ApplicationDbContext>();
            var mapper = new Mock<IMapper>();
            studentService.Setup(_ => _.DeleteStudent(It.IsAny<string>())).ReturnsAsync(false);
            // var sut = new StudentsController(studentService.Object);

            var sut = new StudentsController(dbContext.Object, mapper.Object, studentService.Object);
            var studentId = "testStudentId";

            // Act
            var actionResult = await sut.Delete(studentId);
            var result = actionResult as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

    }
}
