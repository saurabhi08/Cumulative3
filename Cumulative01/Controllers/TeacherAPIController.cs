using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cumulative01.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Cumulative01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TeacherAPIController : ControllerBase
    {

        // Private field to store database context
        private readonly SchoolDbContext _context;


        // Constructor to initialize database context
        public TeacherAPIController(SchoolDbContext context)
        {
            _context = context;
        }
        // GET API to fetch all teachers
        [HttpGet(template:"Teacher")]

        public List<Teacher> RetrieveTeacherList()
        {
            List<Teacher> teachers = new List<Teacher>();
            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();
            Debug.WriteLine("DbConnected");
            string SQLQuery = "SELECT * FROM teachers";
 
            MySqlCommand Command = Connection.CreateCommand();
        
            Command.CommandText = SQLQuery;
            
            MySqlDataReader DataReader = Command.ExecuteReader();

            while (DataReader.Read())
            {
             
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                Teacher newTeacher = new Teacher();
                newTeacher.TeacherId = TeacherId;
                newTeacher.TeacherFirstName = TeacherFName;
                newTeacher.TeacherLastName = TeacherLName;
                newTeacher.EmployeeID = EmployeeID;
                newTeacher.HireDate = HireDate;
                newTeacher.Salary = Salary;
                teachers.Add(newTeacher);
            }
            Connection.Close();
            return teachers;
        }


        // GET API to fetch a teacher by ID
        [HttpGet]
        [Route(template: "SearchTeacher/{id}")]

        public Teacher SearchTeacher(int id)
        {
            Teacher teacher = new Teacher();
            MySqlConnection Connection = _context.GetConnection();
            Connection.Open();

            string SQL = "Select * FROM teachers Where Teacherid = "+id.ToString();
            MySqlCommand Command = Connection.CreateCommand();
            Command.CommandText = SQL;
            MySqlDataReader DataReader = Command.ExecuteReader();
            while (DataReader.Read())
            {
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                teacher.TeacherId = TeacherId;
                teacher.TeacherFirstName = TeacherFName;
                teacher.TeacherLastName = TeacherLName;
                teacher.EmployeeID = EmployeeID;
                teacher.HireDate = HireDate;
                teacher.Salary = Salary;
            }

            Connection.Close(); 
            return teacher;
        }

        // GET API to fetch teachers hired within a specific date range
        [HttpGet]
        [Route(template: "searchbydate")]
        public List<Teacher> searchbydate([FromQuery] string Start, [FromQuery] string End)
        {

            DateTime startDate = DateTime.Parse(Start);
            DateTime endDate = DateTime.Parse(End);

            List<Teacher> teachers = new List<Teacher>();

            MySqlConnection Connection = _context.GetConnection();

            Connection.Open();

            string SQL = "SELECT * FROM `teachers` WHERE teachers.hiredate BETWEEN '"
                  + startDate.ToString("yyyy-MM-dd") + "' AND '"
                  + endDate.ToString("yyyy-MM-dd") + "'";

            MySqlCommand Command = Connection.CreateCommand();

            Command.CommandText = SQL;

            MySqlDataReader DataReader = Command.ExecuteReader();

            while (DataReader.Read())
            {
                int TeacherId = Convert.ToInt32(DataReader["teacherid"]);
                string TeacherFName = DataReader["teacherfname"].ToString();
                string TeacherLName = DataReader["teacherlname"].ToString();
                string EmployeeID = DataReader["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(DataReader["hiredate"]);
                double Salary = Convert.ToDouble(DataReader["salary"]);

                Teacher newTeacher = new Teacher();
                newTeacher.TeacherId = TeacherId;
                newTeacher.TeacherFirstName = TeacherFName;
                newTeacher.TeacherLastName = TeacherLName;
                newTeacher.EmployeeID = EmployeeID;
                newTeacher.HireDate = HireDate;
                newTeacher.Salary = Salary;
                teachers.Add(newTeacher);
            }

            return teachers;

        }

        /// <summary>
        /// Adds a new teacher to the database.
        /// </summary>
        /// <param name="TeacherData">The teacher object containing details to be added.</param>
        /// <returns>ID of the newly added teacher.</returns>
        // API to add a new teacher
        // Example: POST api/TeacherAPI/AddTeacher
        // Body:
        // {
        //   "TeacherFirstName": "Alex",
        //   "TeacherLastName": "Benn",
        //   "EmployeeID": "T378888",
        //   "HireDate": "2016-08-05",
        //   "Salary": 60
        // }
        // Returns: ID of newly added teacher



        [HttpPost(template: "AddTeacher")]
        public int AddTeacher([FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.GetConnection())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = @"INSERT INTO teachers 
            (teacherfname, teacherlname, employeenumber, hiredate, salary) 
            VALUES (@TeacherFName, @TeacherLName, @EmployeeID, @HireDate, @Salary)";

                Command.Parameters.AddWithValue("@TeacherFName", TeacherData.TeacherFirstName);
                Command.Parameters.AddWithValue("@TeacherLName", TeacherData.TeacherLastName);
                Command.Parameters.AddWithValue("@EmployeeID", TeacherData.EmployeeID);
                Command.Parameters.AddWithValue("@HireDate", TeacherData.HireDate.ToString("yyyy-MM-dd"));
                Command.Parameters.AddWithValue("@Salary", TeacherData.Salary);

                Command.ExecuteNonQuery();

                return Convert.ToInt32(Command.LastInsertedId);
            }
            return 0; // In case of failure
        }

        /// <summary>
        /// Deletes a teacher by their ID.
        /// </summary>
        /// <param name="id">ID of the teacher to be deleted.</param>
        /// <returns>Number of rows affected.</returns
        // Example: DELETE api/TeacherAPI/DeleteTeacher/1
       // Deletes teacher with ID 1 and returns number of affected rows


        // DELETE API to remove a teacher by ID
        [HttpDelete(template: "DeleteTeacher/{id}")]
        public int DeleteTeacher(int id)
        {
            using (MySqlConnection Connection = _context.GetConnection())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = "DELETE FROM teachers WHERE teacherid = @TeacherId";
                Command.Parameters.AddWithValue("@TeacherId", id);

                return Command.ExecuteNonQuery(); // Returns number of rows affected
            }
         
        }
        /// <summary>
        /// Updates an existing teacher's information by their ID.
        /// </summary>
        /// <param name="id">The ID of the teacher to update.</param>
        /// <param name="TeacherData">The updated teacher data object received from the request body.</param>
        /// <returns>The updated <see cref="Teacher"/> object.</returns>
        /// <remarks>
        /// Example: PUT api/TeacherAPI/UpdateTeacher/1
        /// Body:
        /// {
        ///     "TeacherFirstName": "John",
        ///     "TeacherLastName": "Doe",
        ///     "EmployeeID": "T123",
        ///     "HireDate": "2023-08-15",
        ///     "Salary": 65000
        /// }
        /// </remarks>


        // PUT API to update an existing teacher
        [HttpPut(template: "UpdateTeacher/{id}")]
        public Teacher UpdateTeacher(int id, [FromBody] Teacher TeacherData)
        {
            using (MySqlConnection Connection = _context.GetConnection())
            {
                Connection.Open();
                MySqlCommand Command = Connection.CreateCommand();

                Command.CommandText = @"UPDATE teachers 
            SET teacherfname = @TeacherFName, 
                teacherlname = @TeacherLName, 
                employeenumber = @EmployeeID, 
                hiredate = @HireDate, 
                salary = @Salary 
            WHERE teacherid = @TeacherId";

                Command.Parameters.AddWithValue("@TeacherFName", TeacherData.TeacherFirstName);
                Command.Parameters.AddWithValue("@TeacherLName", TeacherData.TeacherLastName);
                Command.Parameters.AddWithValue("@EmployeeID", TeacherData.EmployeeID);
                Command.Parameters.AddWithValue("@HireDate", TeacherData.HireDate.ToString("yyyy-MM-dd"));
                Command.Parameters.AddWithValue("@Salary", TeacherData.Salary);
                Command.Parameters.AddWithValue("@TeacherId", id);

                Command.ExecuteNonQuery();
            }

            return SearchTeacher(id); // Reuse the GET by ID to return the updated object
        }





    }
}
