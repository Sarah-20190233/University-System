using AutoMapper;
using Core.InterfacesOfServices;
using Core.InterfacesOfRepo;
using Core.Models;
using Core.Models.DTOs;


namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepo;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepo studentRepo, IMapper mapper)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<List<StudentDto>> GetAllStudents(PaginationParams paginationParams)
        {
            var students = await _studentRepo.GetAll(paginationParams);
            return _mapper.Map<List<StudentDto>>(students);
        }

        public async Task<int> GetStudentCount()
        {
            return await _studentRepo.GetStudentCount();
        }

        public async Task<List<StudentDto>> SearchStudents(string keyword)
        {
            var students = await _studentRepo.SearchStudents(keyword);

            // Map Student entities to StudentDto objects
            var studentDtos = _mapper.Map<List<StudentDto>>(students);

            return studentDtos;
        }

        public async Task<StudentDto> GetStudentById(string id)
        {
            var student = await _studentRepo.GetById(id);
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<bool> AddStudent(StudentDto studentDto)

        {
            var student = _mapper.Map<Student>(studentDto);
            return await _studentRepo.Add(student);
        }

        public async Task<bool> UpdateStudent(string studentId, StudentUpdatedDto studentUpdatedDto)
        {
            var existingStudent = await _studentRepo.GetById(studentId);
            if (existingStudent == null)
                return false;

            _mapper.Map(studentUpdatedDto, existingStudent);

            return await _studentRepo.Update(existingStudent);
        }

        public async Task<bool> DeleteStudent(string id)
        {
            return await _studentRepo.Delete(id);
        }


    }
}
