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

namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GeneralController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<List<Category>> GetCategories()
        {
            List<Category> categories = new();

            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("SELECT * FROM ECM.CATEGORY", conn)
            {
                CommandType = CommandType.Text
            };

            conn.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Category category = new()
                {
                    CategoryId = (int)reader["categoryId"],
                    CategoryType = reader["categoryType"].ToString(),
                    CategoryIcon = reader["categoryIcon"].ToString(),
                    CategoryUrl = reader["categoryUrl"].ToString()
                };
                categories.Add(category);
            }

            command.Dispose();
            reader.Close();

            return categories;
        }
    }
}
