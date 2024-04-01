using Application.Services;
using AutoMapper;
using Core.InterfacesOfRepo;
using Core.Models.DTOs;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AutoBogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace IntegrationTesting.Systems.Services
{
    public class TestStudentService : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly Mock<IStudentRepo> _mockStudentRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly IMapper _mapper;

        public TestStudentService()
        {
            _mockStudentRepo = new Mock<IStudentRepo>();
            _mockMapper = new Mock<IMapper>();
        }


        // Test for GetStudentCount
        [Fact]
        public async Task GetStudentCount_ShouldReturnCount()
        {
            // Arrange
            var expectedCount = 5; // Mock expected count
            _mockStudentRepo.Setup(repo => repo.GetStudentCount()).ReturnsAsync(expectedCount);
            var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

            // Act
            var result = await studentService.GetStudentCount();

            // Assert
            Assert.Equal(expectedCount, result);
        }

        // Test for SearchStudents
        //[Fact]
        //public async Task SearchStudents_ShouldReturnMatchingStudents()
        //{
        //    // Arrange
        //    var keyword = "20242989"; // Provide a search keyword

        //    var expectedStudents = new List<StudentDto>(
        //        {
        //        StudentId = "1002",
        //        Name = "Jane Smith",
        //        Gpa = 3.8,
        //        Level = "Junior"

        //    }); // Mock expected students
        //    _mockStudentRepo.Setup(repo => repo.SearchStudents(keyword)).ReturnsAsync(expectedStudents);
        //    var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

        //    // Act
        //    var result = await studentService.SearchStudents(keyword);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<List<StudentDto>>(result);
        //    // Add more assertions as needed
        //}

        //[Fact]
        //public async Task SearchStudents_ShouldReturnMatchingStudents()
        //{
        //    // Arrange
        //    var keyword = "20242989"; // Provide a search keyword

        //    var expectedStudents = new List<StudentDto>
        //    {
        //       new StudentDto { StudentId = "20242989", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" }
        //    }; // Mock expected students

        //    _mockStudentRepo.Setup(repo => repo.SearchStudents(keyword)).ReturnsAsync(expectedStudents);
        //    var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

        //    // Act
        //    var result = await studentService.SearchStudents(keyword);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<List<StudentDto>>(result);
        //    Assert.Single(result); // Ensure only one student is returned in this case

        //    // You might want to add more specific assertions based on the expected data
        //    Assert.Equal(expectedStudents[0].StudentId, result[0].StudentId);
        //    Assert.Equal(expectedStudents[0].Name, result[0].Name);
        //    Assert.Equal(expectedStudents[0].Gpa, result[0].Gpa);
        //    Assert.Equal(expectedStudents[0].Level, result[0].Level);
        //}

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

        // Test for GetStudentById
        //[Fact]
        //public async Task GetStudentById_ShouldReturnStudent()
        //{
        //    // Arrange
        //    var studentId = "Exclusive"; // Provide a valid student ID
        //    var expectedStudent = new Student(); // Mock expected student
        //    _mockStudentRepo.Setup(repo => repo.GetById(studentId)).ReturnsAsync(expectedStudent);
        //    var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

        //    // Act
        //    var result = await studentService.GetStudentById(studentId);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<StudentDto>(result);
        //    // Add more assertions as needed
        //}

        [Fact]
        public async Task GetStudentById_ShouldReturnMappedStudentDto()
        {
            // Arrange
            var studentId = "20190233"; // Set the student ID
            var expectedStudentDto = new StudentDto { StudentId = "20190233", Name = "Jane Smith", Gpa = 3.8, Level = "Junior" };

            var studentRepoMock = new Mock<IStudentRepo>();
            studentRepoMock.Setup(repo => repo.GetById(studentId)).ReturnsAsync(new Student { StudentId = "20190233", Name = "Jane Smith", Gpa = 3.8, Level = "Junior", DepartmentId = "1" });

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

        // Test for AddStudent
        [Fact]
        public async Task AddStudent_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            //var studentDto = new StudentDto(); // Provide a valid StudentDto
            var studentDto = new AutoFaker<StudentDto>().Generate(); // Generate a random StudentDto using AutoBogus
            var student = new Student(); // Mock Student object
            _mockMapper.Setup(m => m.Map<Student>(studentDto)).Returns(student);
            _mockStudentRepo.Setup(repo => repo.Add(student)).ReturnsAsync(true);
            var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

            // Act
            var result = await studentService.AddStudent(studentDto);

            // Assert
            Assert.True(result);
        }

        // Test for UpdateStudent
        [Fact]
        public async Task UpdateStudent_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            var studentId = "20243740"; // Provide a valid student ID
            var studentUpdatedDto = new StudentUpdatedDto(); // Provide a valid StudentUpdatedDto
            var existingStudent = new Student(); // Mock existing Student object
            _mockStudentRepo.Setup(repo => repo.GetById(studentId)).ReturnsAsync(existingStudent);
            _mockStudentRepo.Setup(repo => repo.Update(existingStudent)).ReturnsAsync(true);
            var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

            // Act
            var result = await studentService.UpdateStudent(studentId, studentUpdatedDto);

            // Assert
            Assert.True(result);
        }

        // Test for DeleteStudent
        [Fact]
        public async Task DeleteStudent_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            var studentId = "20246420"; // Provide a valid student ID
            _mockStudentRepo.Setup(repo => repo.Delete(studentId)).ReturnsAsync(true);
            var studentService = new StudentService(_mockStudentRepo.Object, _mockMapper.Object);

            // Act
            var result = await studentService.DeleteStudent(studentId);

            // Assert
            Assert.True(result);
        }

    }
}
