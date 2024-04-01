using Core.Models;
using Core.Models.DTOs;
using Core.InterfacesOfRepo;

//using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InterfacesOfServices
{
    public interface IStudentService
    {
        Task<List<StudentDto>> GetAllStudents(PaginationParams paginationParams);

        //Task<List<StudentDto>> GetAllStudents();
        Task<int> GetStudentCount();
        Task<List<StudentDto>> SearchStudents(string keyword);
        Task<StudentDto> GetStudentById(string id);
        Task<bool> AddStudent(StudentDto studentDto);
        Task<bool> UpdateStudent(string studentId, StudentUpdatedDto studentUpdatedDto);
        Task<bool> DeleteStudent(string id);


    }
}
