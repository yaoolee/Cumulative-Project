using Cumulative_1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using MySql.Data.MySqlClient;

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
    }
}
