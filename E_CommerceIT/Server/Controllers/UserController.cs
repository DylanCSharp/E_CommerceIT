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

        //VALIDATES USER
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

            //IF RESULT IS 1 THEN THE USER EXISTS AND IS AUTHENTICATED
            if (loginResult == 1)
            {
                SqlCommand commandTwo = new("SELECT userId, isAdmin, isCustomer, userFirstName FROM ECM.USERS WHERE userEmail = '"+user.UserEmail+"'", conn)
                {
                    CommandType = CommandType.Text
                };
                SqlDataReader reader = commandTwo.ExecuteReader();
                while (reader.Read())
                {
                    user.UserId = (int)reader["userId"];
                    user.IsAdmin = (int)reader["isAdmin"];
                    user.IsCustomer = (int)reader["isCustomer"];
                    user.UserFirstName = reader["userFirstName"].ToString();
                }

                commandTwo.Dispose();
                conn.Close();
            }

            GLOBALUSER = user;
            return user;
        }

        //SETTING A GLOBAL USER WHEN THE USER LOGS IN SO THAT WE CAN EASILY GET USER INFO FROM ANYWHERE
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


        //REGISTERING NEW USER WITH HASH AND SALT ALGORITHM IN SQL STORED PROCEDURE
        [HttpPost]
        public Users RegisterUser(Users newUser)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));

            conn.Open();

            SqlCommand command = new("ECM.PROC_REGISTER", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            string password = Encoding.ASCII.GetString(newUser.UserPassword, 0, newUser.UserPassword.Length);

            command.Parameters.AddWithValue("@firstName", newUser.UserFirstName);
            command.Parameters.AddWithValue("@lastName", newUser.UserLastName);
            command.Parameters.AddWithValue("@email", newUser.UserEmail);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@Admin", 0);
            command.Parameters.AddWithValue("@Customer", 1);

            command.ExecuteNonQuery();

            command.Dispose();
            conn.Close();

            ValidateUser(newUser);

            return newUser;
        }


        //GETTING USER DEPENDING ON THEIR EMAIL WHICH IS UNIQUE IN THE DB
        [HttpGet("{email}")]
        public ActionResult<Users> GetUserByEmail(string email)
        {
            Users user = new();

            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("SELECT * FROM ECM.USERS WHERE userEmail = '"+email+"'", conn);
            conn.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                user.UserId = (int)reader["userId"];
                user.UserFirstName = reader["userFirstName"].ToString();
                user.UserLastName = reader["userLastName"].ToString();
                user.UserEmail = reader["userEmail"].ToString();
            }

            command.Dispose();
            reader.Close();
            conn.Close();

            return user;
        }

        //ALLOWING USER TO EDIT THEIR INFO
        [HttpPut("{email}")]
        public Users EditProfile(Users editedUser)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("UPDATE ECM.USERS SET userFirstName = '"+editedUser.UserFirstName+"', userLastName = '"+editedUser.UserLastName+"', userEmail = '"+editedUser.UserEmail+"' WHERE userEmail = '"+editedUser.UserEmail+"'", conn);
            conn.Open();

            command.ExecuteNonQuery();

            command.Dispose();
            conn.Close();

            return editedUser;
        }
    }  
}
