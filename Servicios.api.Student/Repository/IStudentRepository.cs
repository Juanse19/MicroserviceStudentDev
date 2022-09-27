using Servicios.api.Student.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentApi.Repository
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetStudents();
        Task<Student> GetStudentByIdAsync(int Id);
        Task CreateStudent(Student student);
        Task UpdateStudent(Student student);
        Task<Student> DeleteStudent(int Id);
    }
}
