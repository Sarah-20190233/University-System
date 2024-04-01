using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Core.Models.DTOs;
using Core.Models;
using Core.InterfacesOfServices;
using Microsoft.AspNetCore.Authorization;
namespace UnversityManagement.Controllers
{
    //[Authorize]
    [Route("api/StudentsController")]
    //[EnableCors("AllowOrigin")]
    [ApiController]
 
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        //private readonly IReposirty<Student> _studentRepositry;
        private readonly IMapper _mapper; // Add IMapper field


        private readonly IStudentService _studentService;

        //public StudentsController(
        //  IStudentService studentService)
        //{

        //    _studentService = studentService;

        //}
        public StudentsController(ApplicationDbContext context,
            IMapper mapper,
            IStudentService studentService)
        {
            _context = context;
            _mapper = mapper;
            _studentService = studentService;

        }


        // To get the students' data
        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> GetStudents([FromQuery] PaginationParams paginationParams)
        {
            var students = await _studentService.GetAllStudents(paginationParams);
 
            if (students.Any())
            {
                return Ok(students);
            }
            else
            {
                return NoContent(); // Return 204 No Content when no students are retrieved
            }
        }

        //[HttpGet]
        //public async Task<ActionResult<List<StudentDto>>> GetStudents()
        //{
        //    var students = await _studentService.GetAllStudents();
        //    return Ok(students);
        //}


        [HttpGet("count")]
        public async Task<ActionResult<int>> GetStudentCount()
        {
            try
            {
                var studentCount = await _studentService.GetStudentCount();

                if(studentCount == 0)
                {
                    return NoContent();
                }

                else
                {


                    return Ok(studentCount);
                    
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<Student>>> SearchStudents(string keyword)
        {
            try
            {
                var students = await _studentService.SearchStudents(keyword);

                if (students.Any())
                {
                    return Ok(students);
                }
                else
                {
                    return NotFound(); // Return 404 Not Found if no students match the search criteria
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // To get student data based on student Id

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var student = await _studentService.GetStudentById(id);
            if (student == null)
                return NotFound();

            return Ok(student);
        }



        [HttpPost]
        public async Task<IActionResult> Post(StudentDto studentDto)
        {
            var result = await _studentService.AddStudent(studentDto);
            if (result)
                return Ok();
            else
                return StatusCode(500, "Failed to add student."); // Handle failure case
        }


        [HttpPut("{studentId}")]
        public async Task<IActionResult> Put(string studentId, StudentUpdatedDto studentUpdatedDto)
        {
            var result = await _studentService.UpdateStudent(studentId, studentUpdatedDto);
            if (result)
                return Ok();
            else
                return NotFound(); // Or any appropriate error response
        }

       


        // To delete a student data based on student id
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string Id)
        {
            var result = await _studentService.DeleteStudent(Id);
            if (result)
                return Ok();
            else
                return NotFound(); // Or any appropriate error response
        }



    }

}
