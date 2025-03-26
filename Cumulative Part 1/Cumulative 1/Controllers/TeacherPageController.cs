using Microsoft.AspNetCore.Mvc;
using Cumulative_1.Models;
using MySql.Data.MySqlClient;


namespace Cumulative_1.Controllers
{
    public class TeacherPageController : Controller
    {
        private readonly SchoolDbContext _dbContext;

        public TeacherPageController(SchoolDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult List()
        {
            List<Teacher> teachers = new List<Teacher>();

            using (MySqlConnection conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM teachers", conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        teachers.Add(new Teacher
                        {
                            TeacherId = reader.GetInt32("teacherid"),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString(),
                            EmployeeNumber = reader["employeenumber"].ToString(),
                            HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime("hiredate")
                        });
                    }
                }
            }

            return View(teachers);
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Teacher teacher = null;

            using (MySqlConnection conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM teachers WHERE teacherid = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            teacher = new Teacher
                            {
                                TeacherId = reader.GetInt32("teacherid"),
                                TeacherFName = reader["teacherfname"].ToString(),
                                TeacherLName = reader["teacherlname"].ToString(),
                                EmployeeNumber = reader["employeenumber"].ToString(),
                                HireDate = reader.IsDBNull(reader.GetOrdinal("hiredate"))
                                            ? (DateTime?)null
                                            : reader.GetDateTime("hiredate")
                            };
                        }
                    }
                }
            }

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher); 
        }
    }
}
