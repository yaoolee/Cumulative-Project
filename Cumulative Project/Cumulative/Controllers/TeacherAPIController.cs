using Cumulative_1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using MySql.Data.MySqlClient;
using System.Reflection.Metadata.Ecma335;

namespace Cumulative_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAPIController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Returns a list of Teachers in the system. If a search key is included, searches for teachers with a first or last name matching the key.
        /// </summary>
        /// <example>
        /// GET: api/TeacherAPI/ListTeachers?SearchKey=John -> [{"TeacherId":1,"TeacherFName":"John", "TeacherLName":"Smith"},{"TeacherId":2,"TeacherFName":"Johnny", "TeacherLName":"Doe"},..]
        /// </example>
        /// <returns>
        /// A list of Teacher objects
        /// </returns>

        [HttpGet]
        [Route("ListTeachers")]
        public List<Teacher> ListTeachers(string SearchKey = null)
        {
            List<Teacher> Teachers = new List<Teacher>();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                string query = "SELECT * FROM teachers";

                if (SearchKey != null)
                {
                    query += " WHERE LOWER(teacherfname) LIKE @key OR LOWER(teacherlname) LIKE @key OR LOWER(CONCAT(teacherfname, ' ', teacherlname)) LIKE @key";
                    Command.Parameters.AddWithValue("@key", $"%{SearchKey.ToLower()}%");
                }

                Command.CommandText = query;
                Command.Prepare();

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        Teachers.Add(new Teacher
                        {
                            TeacherId = Convert.ToInt32(ResultSet["teacherid"]),
                            TeacherFName = ResultSet["teacherfname"].ToString(),
                            TeacherLName = ResultSet["teacherlname"].ToString(),
                            EmployeeNumber = ResultSet["employeenumber"].ToString(),
                            HireDate = Convert.ToDateTime(ResultSet["hiredate"])
                        });
                    }
                }
            }

            return Teachers;
        }
        /// <summary>
        /// Returns a teacher in the database by their ID.
        /// </summary>
        /// <example>
        /// GET: api/TeacherAPI/FindTeacher/2 -> {"TeacherId":2,"TeacherFName":"John","TeacherLName":"Smith","EmployeeNumber":"T123","HireDate":"2023-09-01T00:00:00","Salary":60000}
        /// </example>
        /// <returns>
        /// A matching Teacher object by its ID. Empty object if Teacher not found.
        /// </returns>

        [HttpGet]
        [Route("FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = new Teacher();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", id);

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        SelectedTeacher.TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                        SelectedTeacher.TeacherFName = ResultSet["teacherfname"].ToString();
                        SelectedTeacher.TeacherLName = ResultSet["teacherlname"].ToString();
                        SelectedTeacher.EmployeeNumber = ResultSet["employeenumber"].ToString();
                        SelectedTeacher.HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                    }
                }
            }

            return SelectedTeacher;
        }
        /// <summary>
        /// Adds a Teacher to the database.
        /// </summary>
        /// <param name="TeacherData">Teacher object containing the new teacher's information</param>
        /// <example>
        /// POST: api/TeacherAPI/AddTeacher
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///     "TeacherFName": "John",
        ///     "TeacherLName": "Smith",
        ///     "EmployeeNumber": "T123",
        ///     "HireDate": "2023-09-01T00:00:00",
        ///     "Salary": 60000
        /// } -> 201
        /// </example>
        /// <returns>
        /// The inserted Teacher Id from the database if successful; 0 if unsuccessful.
        /// </returns>

        [HttpPost]
        [Route("AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = @"INSERT INTO teachers 
                                        (teacherfname, teacherlname, employeenumber, hiredate) 
                                        VALUES (@fname, @lname, @emp, CURRENT_DATE())";

                Command.Parameters.AddWithValue("@fname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@lname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@emp", TeacherData.EmployeeNumber);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);
            }

            return 0;
        }
        /// <summary>
        /// Deletes a Teacher from the database.
        /// </summary>
        /// <param name="TeacherId">Primary key of the teacher to delete</param>
        /// <example>
        /// DELETE: api/TeacherAPI/DeleteTeacher/1
        /// </example>
        /// <returns>
        /// Number of rows affected by the delete operation.
        /// </returns>

        [HttpDelete]
        [Route("DeleteTeacher/{TeacherId}")]
        public int DeleteTeacher(int TeacherId)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                Command.Parameters.AddWithValue("@id", TeacherId);

                return Command.ExecuteNonQuery();
            }

            return 0;
        }

        /// <summary>
        /// Updates a Teacher in the database. Data is a Teacher object, request query contains the ID.
        /// </summary>
        /// <param name="TeacherData">Teacher object</param>
        /// <param name="TeacherId">The Teacher ID primary key</param>
        /// <example>
        /// PUT: api/TeacherAPI/UpdateTeacher/3  
        /// Headers: Content-Type: application/json  
        /// Request Body:
        /// {
        ///     "TeacherFName": "John",
        ///     "TeacherLName": "Doe",
        ///     "EmployeeNumber": "T456",
        ///     "HireDate": "2020-09-01",
        ///     "Salary": 62000.50
        /// }
        /// ->
        /// {
        ///     "TeacherId": 3,
        ///     "TeacherFName": "John",
        ///     "TeacherLName": "Doe",
        ///     "EmployeeNumber": "T456",
        ///     "HireDate": "2020-09-01T00:00:00",
        ///     "Salary": 62000.50
        /// }
        /// </example>
        /// <returns>The updated Teacher object</returns>
        [HttpPut("UpdateTeacher/{TeacherId}")]
        public Teacher UpdateTeacher(int TeacherId, [FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = @"UPDATE teachers 
                                SET teacherfname = @teacherfname, 
                                    teacherlname = @teacherlname, 
                                    employeenumber = @employeenumber, 
                                    hiredate = @hiredate, 
                                    salary = @salary 
                                WHERE teacherid = @id";

                Command.Parameters.AddWithValue("@teacherfname", TeacherData.TeacherFName);
                Command.Parameters.AddWithValue("@teacherlname", TeacherData.TeacherLName);
                Command.Parameters.AddWithValue("@employeenumber", TeacherData.EmployeeNumber);
                Command.Parameters.AddWithValue("@hiredate", TeacherData.HireDate);
                Command.Parameters.AddWithValue("@salary", TeacherData.Salary);
                Command.Parameters.AddWithValue("@id", TeacherId);

                Command.ExecuteNonQuery();
            }

            return FindTeacher(TeacherId);
        }
    }
}
