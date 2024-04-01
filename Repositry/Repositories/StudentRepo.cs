using AutoMapper;
using Core;
using Core.InterfacesOfRepo;
using Core.Models;
using Core.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StudentRepo : BaseRepo<Student>, IStudentRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public StudentRepo(ApplicationDbContext context, IMapper mapper): base(context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> GetStudentCount()
        {
            return await _context.Students.CountAsync();
        }

        public async Task<List<Student>> SearchStudents(string keyword)
        {
            var students = await _context.Students
                .Where(s => s.Name.Contains(keyword) || s.StudentId.ToString() == keyword
                     || s.Gpa.ToString() == keyword || s.Level.ToString() == keyword)
                .ToListAsync();

            return students;
        
        }
    }
}
