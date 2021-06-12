using E_CommerceIT.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<List<Product>> GetAllProducts()
        {
            List<Product> products = new();

            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("SELECT * FROM ECM.PRODUCTS", conn)
            {
                CommandType = CommandType.Text
            };
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                Product product = new()
                {
                    ProductId = (int)reader["productId"],
                    ProductName = reader["productName"].ToString(),
                    ProductDescription = reader["productDescription"].ToString(),
                    ProductImage = reader["productImage"].ToString(),
                    ProductPrice = (decimal)reader["productPrice"],
                    ProductCreated = (DateTime)reader["productCreated"],
                    CategoryId = (int)reader["categoryId"]
                };
                products.Add(product);
            }

            command.Dispose();
            reader.Close();

            return products;
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductById(int id)
        {
            Product product = new();

            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("SELECT * FROM ECM.PRODUCTS WHERE productId = " + id + "", conn)
            {
                CommandType = CommandType.Text
            };

            conn.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                product.ProductId = (int)reader["productId"];
                product.ProductName = reader["productName"].ToString();
                product.ProductDescription = reader["productDescription"].ToString();
                product.ProductImage = reader["productImage"].ToString();
                product.ProductPrice = (decimal)reader["productPrice"];
                product.ProductCreated = (DateTime)reader["productCreated"];
                product.CategoryId = (int)reader["categoryId"];
            }

            command.Dispose();
            conn.Close();

            return product;
        }

        [HttpGet("{categoryUrl}")]
        public ActionResult<List<Product>> GetProductPerCategory(string categoryurl)
        {
            List<Product> products = new();
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("SELECT ECM.PRODUCTS.productId, ECM.PRODUCTS.productName, ECM.PRODUCTS.productDescription, ECM.PRODUCTS.productImage, ECM.PRODUCTS.productPrice, ECM.PRODUCTS.productCreated FROM ECM.PRODUCTS INNER JOIN ECM.CATEGORY ON ECM.PRODUCTS.categoryId = ECM.CATEGORY.categoryId where ecm.CATEGORY.categoryUrl = '"+categoryurl+"'", conn)
            {
                CommandType = CommandType.Text
            };

            conn.Open();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Product product = new()
                {
                    ProductId = (int)reader["productId"],
                    ProductName = reader["productName"].ToString(),
                    ProductDescription = reader["productDescription"].ToString(),
                    ProductImage = reader["productImage"].ToString(),
                    ProductPrice = (decimal)reader["productPrice"],
                    ProductCreated = (DateTime)reader["productCreated"]
                };
                products.Add(product);
            }

            categoryurl = null;

            return products;
        }
    }
}
