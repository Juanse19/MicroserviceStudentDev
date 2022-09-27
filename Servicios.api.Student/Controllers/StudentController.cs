using Microsoft.AspNetCore.Mvc;
using StudentApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Student.Controllers
{
    [Route("api/student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Core.Entities.Student>>> Get()
        {
            var result = await _studentRepository.GetStudents();
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Core.Entities.Student>> GetById(int Id)
        {
            var result = await _studentRepository.GetStudentByIdAsync(Id);
            return Ok(result);
        }

        [HttpPost]
        public async Task Post([FromBody] Core.Entities.Student student)
        {
            if (ModelState.IsValid)
                await _studentRepository.CreateStudent(student);
        }

        [HttpPut("{Id:int}")]
        public async Task Put(int Id, [FromBody] Core.Entities.Student student)
        {
            student.Id = Id;
            if (ModelState.IsValid)
                await _studentRepository.UpdateStudent(student);

        }

        [HttpDelete("{id:int}")]
        public async Task Delete(int Id)
        {
            var result = await _studentRepository.DeleteStudent(Id);
        }

    }
}
