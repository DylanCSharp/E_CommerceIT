using E_CommerceIT.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{userId}")]
        public ActionResult<List<CustomOrderHistory>> GetOrderHistoryForUser(string userId)
        {
            List<CustomOrderHistory> orders = new();
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new(" SELECT ECM.ORDER_HISTORY.orderNum, ECM.PRODUCTS.productName, ECM.PRODUCTS.productImage, ECM.CATEGORY.categoryType, ECM.ORDER_HISTORY.orderDate, ECM.ORDER_HISTORY.orderYear, ECM.PRODUCTS.productPrice FROM ECM.PRODUCTS INNER JOIN ECM.CATEGORY ON ECM.PRODUCTS.categoryId = ECM.CATEGORY.categoryId INNER JOIN ECM.ORDER_HISTORY ON ECM.PRODUCTS.productId = ECM.ORDER_HISTORY.productId AND ECM.CATEGORY.categoryId = ECM.ORDER_HISTORY.categoryId INNER JOIN ECM.USERS ON ECM.ORDER_HISTORY.userId = ECM.USERS.userId where ECM.USERS.userId = " + Convert.ToInt32(userId)+"", conn);
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CustomOrderHistory history = new();

                history.OrderNum = (int)reader["orderNum"];
                history.ProductName = reader["productName"].ToString();
                history.ProductImage = reader["productImage"].ToString();
                history.CategoryType = reader["categoryType"].ToString();
                history.OrderDate = (int)reader["orderDate"];
                history.OrderYear = (int)reader["orderYear"];
                history.ProductPrice = (decimal)reader["productPrice"];

                switch (history.OrderDate)
                {
                    case 1:
                        history.Month = "January";
                        break;
                    case 2:
                        history.Month = "February";
                        break;
                    case 3:
                        history.Month = "March";
                        break;
                    case 4:
                        history.Month = "April";
                        break;
                    case 5:
                        history.Month = "May";
                        break;
                    case 6:
                        history.Month = "June";
                        break;
                    case 7:
                        history.Month = "July";
                        break;
                    case 8:
                        history.Month = "August";
                        break;
                    case 9:
                        history.Month = "September";
                        break;
                    case 10:
                        history.Month = "October";
                        break;
                    case 11:
                        history.Month = "November";
                        break;
                    case 12:
                        history.Month = "December";
                        break;
                }

                orders.Add(history);
            }

            command.Dispose();
            conn.Close();

            return orders;
        }

        [HttpPost("")]
        public CartItem FinalCheckOut(CartItem cartItem)
        {
            int id = UserController.GLOBALUSER.UserId;

            SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));

            conn.Open();

            SqlCommand command = new("SELECT ECM.CATEGORY.categoryId FROM ECM.PRODUCTS INNER JOIN ECM.CATEGORY ON ECM.PRODUCTS.categoryId = ECM.CATEGORY.categoryId where productId = "+cartItem.ProductId+"", conn);
            int categoryId = (int)command.ExecuteScalar();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            command.Dispose();

            SqlCommand commandTwo = new("INSERT INTO ECM.ORDER_HISTORY VALUES ("+cartItem.ProductId+", "+id+", "+categoryId+", "+month+", "+year+")", conn);
            commandTwo.ExecuteNonQuery();
            commandTwo.Dispose();

            conn.Close();

            return cartItem;
        }
    }
}
