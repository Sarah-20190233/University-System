using Core.Models;
using Core.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityTest.MockData
{
    public class StudentMockData
    {
        public static List<StudentDto> GetStudents()
        {
            return new List<StudentDto>
            {
                new StudentDto
                {
                    StudentId = "1001",
                    Name = "John Doe",
                    Gpa = 3.5,
                    Level = "Senior"
                
                },

                new StudentDto
                {
                    StudentId = "1002",
                    Name = "Jane Smith",
                    Gpa = 3.8,
                    Level = "Junior"
                }

            };
        }


        public static List<StudentDto> GetEmptyStudents()
        {
            return new List<StudentDto>
            {
               

            };
        }

    }

 }
