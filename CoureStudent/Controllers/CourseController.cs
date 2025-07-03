using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoureStudent.Model;
using CoureStudent.Context;

namespace CoureStudent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CourseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            try
            {
                var courses = await _context.Courses.Include(c => c.Students).ToListAsync();
                return Ok(courses);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong while retrieving courses");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Курс не может быть пустым");
            }
            try
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return Ok(new { id = course.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка создания курса: {ex.Message}");
            }
        }

        [HttpPost("{id:guid}/students")]
        public async Task<IActionResult> AddStudent(Guid id, [FromBody] Student student)
        {
            if (student == null)
                return BadRequest("Студент не может быть пустым");

            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course == null) return NotFound("Курс не найден");

                student.CourseId = id;  // исправлено
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return Ok(new { id = student.Id });
            }
            catch (Exception)
            {
                return StatusCode(500, "Ошибка добавления студента");
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            try
            {
                var course = await _context.Courses.Include(c => c.Students)
                                        .FirstOrDefaultAsync(c => c.Id == id);
                if (course == null) return NotFound("Курс не найден");

                _context.Students.RemoveRange(course.Students);
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Ошибка удаления курса");
            }
        }
    }
}
