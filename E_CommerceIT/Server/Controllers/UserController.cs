using E_CommerceIT.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

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
                    user.isAdmin = (int)reader["isAdmin"];
                    user.isCustomer = (int)reader["isCustomer"];
                }

                commandTwo.Dispose();
                conn.Close();

                if (user.isAdmin == 1 && user.isCustomer == 0)
                {
                    HttpContext.Session.SetString("isAdmin", user.UserId.ToString());
                }
                else if (user.isAdmin == 0 && user.isCustomer == 1)
                {
                    HttpContext.Session.SetString("isCustomer", user.UserId.ToString());
                }
            }

            return user;
        }
    }
}
