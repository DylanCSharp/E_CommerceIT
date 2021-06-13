using E_CommerceIT.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Text;


namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public static Users GLOBALUSER { get; set; } = new();

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public Users ValidateUser(Users user)
        {

            string password = Encoding.ASCII.GetString(user.UserPassword, 0, user.UserPassword.Length);
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("ECM.PROC_LOGIN", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            conn.Open();

            command.Parameters.AddWithValue("@email", user.UserEmail);
            command.Parameters.AddWithValue("@password", password);

            int loginResult = (int)command.ExecuteScalar();

            command.Dispose();

            if (loginResult == 1)
            {
                SqlCommand commandTwo = new("SELECT userId, isAdmin, isCustomer FROM ECM.USERS WHERE userEmail = '"+user.UserEmail+"'", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = commandTwo.ExecuteReader();
                while (reader.Read())
                {
                    user.UserId = (int)reader["userId"];
                    user.IsAdmin = (int)reader["isAdmin"];
                    user.IsCustomer = (int)reader["isCustomer"];
                }

                commandTwo.Dispose();
                conn.Close();
            }

            GLOBALUSER = user;
            return user;
        }

        [HttpGet]
        public ActionResult<Users> GetGlobalUser()
        {
            return GLOBALUSER;
        }

        [HttpPost]
        public Users ClearUser()
        {
            GLOBALUSER = new();

            return GLOBALUSER;
        }
    }  
}
