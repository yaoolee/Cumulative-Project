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
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers(string SearchKey = null)
        {
           
            List<Teacher> Teachers = new List<Teacher>();

            
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
              
                MySqlCommand Command = Connection.CreateCommand();


                string query = "select * from teachers";

                if (SearchKey != null)
                {
                    query += " where lower(teacherfname) like @key or lower(teacherlname) like @key or lower(concat(teacherfname,' ',teacherlname)) like @key";
                    Command.Parameters.AddWithValue("@key", $"%{SearchKey}%");
                }
             
                Command.CommandText = query;
                Command.Prepare();

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();

                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string TeacherNumber = ResultSet["employeenumber"].ToString();

                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            HireDate = TeacherHireDate,
                            EmployeeNumber = TeacherNumber
                        };

                        Teachers.Add(CurrentTeacher);

                    }
                }
            }

            return Teachers;
        }
        /// <summary>
        /// Returns information on all Teachers
        /// </summary>
        /// <example>
        /// GET api/TeacherAPI/ListTeachers -> [{"teacherId":1, "teacherFname":"Alexander", "teacherLname":"Bennett", "salary":null,..}]
        /// </example>
        /// <returns>
        /// list of strings displaying all teachers with information
        /// </returns>
        [HttpGet]
        [Route(template: "ListTeachers")]
        public List<Teacher> ListTeachers()
        {
            List<Teacher> Teachers = new List<Teacher>();
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from teachers";

                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();

                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string TeacherNumber = ResultSet["employeenumber"].ToString();

                        Teacher CurrentTeacher = new Teacher()
                        {
                            TeacherId = Id,
                            TeacherFName = FirstName,
                            TeacherLName = LastName,
                            HireDate = TeacherHireDate,
                            EmployeeNumber = TeacherNumber
                        };

                        Teachers.Add(CurrentTeacher);

                    }
                }
            }

            return Teachers;
        }
        /// <summary>
        /// Returns all information on a teacher using their specific ID
        /// </summary>
        /// <example>
        /// GET api/TeacherAPI/FindTeacher/4 -> [{"teacherId":4, "teacherFname":"Lauren", "teacherLname":"Smith", "salary":null,..}]
        /// </example>
        /// <returns>
        /// A list of strings of selected teacher having all of the information
        /// </returns>

        [HttpGet]
        [Route(template: "FindTeacher/{id}")]
        public Teacher FindTeacher(int id)
        {
            Teacher SelectedTeacher = new Teacher();

            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();
                Command.CommandText = "select * from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader ResultSet = Command.ExecuteReader())
                {
                    while (ResultSet.Read())
                    {
                        int Id = Convert.ToInt32(ResultSet["teacherid"]);
                        string FirstName = ResultSet["teacherfname"].ToString();
                        string LastName = ResultSet["teacherlname"].ToString();

                        DateTime TeacherHireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                        string TeacherNumber = ResultSet["employeenumber"].ToString();

                        SelectedTeacher.TeacherId = Id;
                        SelectedTeacher.TeacherFName = FirstName;
                        SelectedTeacher.TeacherLName = LastName;
                        SelectedTeacher.EmployeeNumber = TeacherNumber;
                        SelectedTeacher.HireDate = TeacherHireDate;
                    }
                }
            }


            return SelectedTeacher;
        }
        /// <summary>
        /// Adds teacher to the database
        /// </summary>
        /// <param name="TeacherAPI">Teacher Object</param>
        /// <example>
        /// POST: api/TeacherAPI/AddTeacher
        /// Headers: Content-Type: application/json
        /// Request Body:
        /// {
        ///	    "TeacherFname":"Willy",
        ///	    "TeacherLname":"Wonka",
        ///	    "EmployeeNumber":"T5678"
        /// } -> 16
        /// </example>
        /// <returns>
        /// The inserted teacher Id from the database if successful. 0 if Unsuccessful
        /// </returns>
        [HttpPost(template:"AddTeacher")]
        public ActionResult<int> AddTeacher([FromBody] Teacher TeacherData)
        {
            using (MySqlConnection conn = _context.AccessDatabase())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate) VALUES (@fname, @lname, @number, CURRENT_DATE())";
                cmd.Parameters.AddWithValue("@fname", TeacherData.TeacherFName);
                cmd.Parameters.AddWithValue("@lname", TeacherData.TeacherLName);
                cmd.Parameters.AddWithValue("@number", TeacherData.EmployeeNumber);

                cmd.ExecuteNonQuery();
                return Ok(Convert.ToInt32(cmd.LastInsertedId));
            }
            return 0;
        }
        /// <summary>
        /// Deletes teacher from the database
        /// </summary>
        /// <param name="id">Primary key of the teacher to delete</param>
        /// <example>
        /// DELETE: api/TeacherAPI/DeleteTeacher -> 1
        /// </example>
        /// <returns>
        /// Number of rows affected by delete operation.
        /// </returns>
        [HttpDelete(template: "DeleteTeacher/{id}")]
        public int DeleteTeacher(int id)
        {
            using (MySqlConnection Connection = _context.AccessDatabase())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();


                Command.CommandText = "delete from teachers where teacherid=@id";
                Command.Parameters.AddWithValue("@id", id);
                return Command.ExecuteNonQuery();

            }
            return 0;
        }


    }

}
