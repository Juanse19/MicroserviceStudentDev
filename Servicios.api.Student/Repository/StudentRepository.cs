using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Servicios.api.Student.Core;
using StudentApi.Repository;
using System;
using System.Collections.Generic;
using Servicios.api.Student.Core.Entities;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Servicios.api.Student.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private string conStr;
        public StudentRepository(IOptions<SqliteSettings> options)
        {
            //conStr = @"Data Source=C:/Users/jlosada/Downloads/sample.db3";
            conStr = options.Value.ConnectionString;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqliteConnection(conStr);
            }
        }

        /*
         * INSERT STUDENT
         */
        public async Task CreateStudent(Core.Entities.Student student)
        {
            using (IDbConnection dbconnection = Connection)
            {
                string sql = @"INSERT INTO Student (  Username, FirstName, LastName, Age, Career ) VALUES (  @Username, @FirstName, @LastName, @Age, @Career);";
                dbconnection.Open();

                await dbconnection.ExecuteAsync(sql, student);
            }
        }

        /*
         * DELETE STUDENT
         */
        public async Task<Core.Entities.Student> DeleteStudent(int Id)
        {
            using (IDbConnection dbconnection = Connection)
            {
                dbconnection.Open();
                var result = await dbconnection.QueryAsync<Core.Entities.Student>("DELETE FROM Student WHERE Id = @Id", new { Id = Id });
                return result.FirstOrDefault();
            }
        }

        /*
         * GET BY ID STUDENT
         */
        public async Task<Core.Entities.Student> GetStudentByIdAsync(int Id)
        {
            using (IDbConnection dbconnection = Connection)
            {
                dbconnection.Open();
                var result = await dbconnection.QueryAsync<Core.Entities.Student>("SELECT * FROM Student WHERE Id=@Id", new { Id = Id });

                return result.FirstOrDefault();
            }
        }

        /*
         * GET ALL STUDENTS
         */
        public async Task<IEnumerable<Core.Entities.Student>> GetStudents()
        {
            using (IDbConnection dbconnection = Connection)
            {
                dbconnection.Open();
                return (IEnumerable<Core.Entities.Student>)await dbconnection.QueryAsync<Core.Entities.Student>("SELECT * FROM Student");
            }
        }

        /*
         * UPDATE STUDENT
         */
        public async Task UpdateStudent(Core.Entities.Student student)
        {
            try
            {
                using (IDbConnection dbconnection = Connection)
                {
                    string sql = @"UPDATE Student SET Username = @Username, FirstName = @FirstName, LastName = @LastName, Age = @Age, Career = @Career WHERE Id = @Id";
                    dbconnection.Open();
                    await dbconnection.ExecuteAsync(sql, student);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
