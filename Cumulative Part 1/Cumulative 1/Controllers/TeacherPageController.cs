using Microsoft.AspNetCore.Mvc;
using Cumulative_1.Models;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;


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
        // GET : TeacherPage/New
        [HttpGet]
        public IActionResult New(int id)
        {
            return View();
        }
        // POST: TeacherPage/Create
        [HttpPost]
        public IActionResult Create(Teacher teacher)
        {
            using (MySqlConnection conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate) VALUES (@fname, @lname, @empnum, CURRENT_DATE())", conn))
                {
                    cmd.Parameters.AddWithValue("@fname", teacher.TeacherFName);
                    cmd.Parameters.AddWithValue("@lname", teacher.TeacherLName);
                    cmd.Parameters.AddWithValue("@empnum", teacher.EmployeeNumber);

                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult DeleteConfirm(int id)
        {
            Teacher teacher = null;
            using (MySqlConnection conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM teachers WHERE teacherid = @id";
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teacher = new Teacher
                        {
                            TeacherId = reader.GetInt32("teacherid"),
                            TeacherFName = reader["teacherfname"].ToString(),
                            TeacherLName = reader["teacherlname"].ToString()
                        };
                    }
                }
            }

            if (teacher == null) return NotFound();

            return View(teacher);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (MySqlConnection conn = _dbContext.AccessDatabase())
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM teachers WHERE teacherid = @id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("List");
        }
    }
}
