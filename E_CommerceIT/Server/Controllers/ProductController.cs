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
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blazored.Toast.Services;

namespace E_CommerceIT.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private static string URL;
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
            while (reader.Read())
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
            SqlCommand command = new("SELECT ECM.PRODUCTS.productId, ECM.PRODUCTS.productName, ECM.PRODUCTS.productDescription, ECM.PRODUCTS.productImage, ECM.PRODUCTS.productPrice, ECM.PRODUCTS.productCreated FROM ECM.PRODUCTS INNER JOIN ECM.CATEGORY ON ECM.PRODUCTS.categoryId = ECM.CATEGORY.categoryId where ecm.CATEGORY.categoryUrl = '" + categoryurl + "'", conn)
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

        [HttpGet("{category}/{year}")]
        public async Task<ActionResult<List<SplineChartData>>> GetSplineData(string category, string year)
        {
            List<SplineChartData> splineData = new();

            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            await conn.OpenAsync();
            SqlCommand command = new();
            for (int month = 1; month <= 12; month++)
            {
                command = new("SELECT COUNT(ECM.ORDER_HISTORY.categoryId) as Count from ECM.ORDER_HISTORY INNER JOIN ECM.CATEGORY ON ECM.ORDER_HISTORY.categoryId = ECM.CATEGORY.categoryId WHERE orderDate = " + month + " AND categoryType = '" + category + "' AND orderYear = "+Convert.ToInt32(year)+"", conn)
                {
                    CommandType = CommandType.Text
                };

                SplineChartData chartData = new();
                chartData.Category = category;
                chartData.Count = (int)await command.ExecuteScalarAsync();
                switch (month)
                {
                    case 1:
                        chartData.Month = "January";
                        break;
                    case 2:
                        chartData.Month = "February";
                        break;
                    case 3:
                        chartData.Month = "March";
                        break;
                    case 4:
                        chartData.Month = "April";
                        break;
                    case 5:
                        chartData.Month = "May";
                        break;
                    case 6:
                        chartData.Month = "June";
                        break;
                    case 7:
                        chartData.Month = "July";
                        break;
                    case 8:
                        chartData.Month = "August";
                        break;
                    case 9:
                        chartData.Month = "September";
                        break;
                    case 10:
                        chartData.Month = "October";
                        break;
                    case 11:
                        chartData.Month = "November";
                        break;
                    case 12:
                        chartData.Month = "December";
                        break;
                }

                splineData.Add(chartData);
            };

            await command.DisposeAsync();
            await conn.CloseAsync();

            return splineData;
        }

        [HttpPost]
        public Product AddProduct(Product newProduct)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("ECommerceDB"));
            SqlCommand command = new("INSERT INTO ECM.PRODUCTS VALUES('"+newProduct.ProductName+"', '"+newProduct.ProductDescription+"', '"+URL+"', "+newProduct.ProductPrice+", GETDATE(), "+newProduct.CategoryId+")", conn);
            conn.Open();
            command.ExecuteNonQuery();
            URL = null;
            command.Dispose();
            conn.Close();
            
            return newProduct;
        }

        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var productImage = formCollection.Files[0];
                if (productImage.Length > 0)
                {
                    var container = new BlobContainerClient(_configuration.GetConnectionString("AzureConnectionString"), "productcontainer");
                    var createResponse = container.CreateIfNotExists();
                    if (createResponse != null && createResponse.GetRawResponse().Status == 201)
                    {
                        container.SetAccessPolicy(PublicAccessType.Blob);
                    }
                    var blob = container.GetBlobClient(productImage.FileName);
                    blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
                    using (var fileStream = productImage.OpenReadStream())
                    {
                        blob.Upload(fileStream, new BlobHttpHeaders { ContentType = productImage.ContentType });
                    }

                    URL = "https://ecommerceit.blob.core.windows.net/productcontainer/" + productImage.FileName;

                    return Ok(blob.Uri.ToString());
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}

