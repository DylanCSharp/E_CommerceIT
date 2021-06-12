using Blazored.LocalStorage;
using Blazored.Toast.Services;
using E_CommerceIT.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace E_CommerceIT.Client.Shared
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IToastService _toastService;
        private readonly HttpClient _http;

        public event Action OnChange;

        public CartService(ILocalStorageService localStorage, IToastService toastService, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _toastService = toastService;
            _http = httpClient;
        }

        public async Task AddToCart(Product product)
        {
            var cart = await _localStorage.GetItemAsync<List<Product>>("cart");
            if (cart == null)
            {
                cart = new List<Product>();
            }

            cart.Add(product);
            await _localStorage.SetItemAsync("cart", cart);
            _toastService.ShowSuccess("Added:" + product.ProductName, "Successfully Updated Cart!");

            OnChange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var result = new List<CartItem>();
            var cart = await _localStorage.GetItemAsync<List<Product>>("cart");
            if (cart == null)
            {
                return result = new List<CartItem>();
            }
            foreach (var item in cart)
            {
                var product = await _http.GetFromJsonAsync<Product>("Product/GetProductById/" +item.ProductId.ToString());
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductImage = product.ProductImage,
                    ProductPrice = product.ProductPrice
                };

                result.Add(cartItem);

            }

            return result;
        }

        public async Task DeleteItem(CartItem item)
        {
            var cart = await _localStorage.GetItemAsync<List<Product>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == item.ProductId);
            cart.Remove(cartItem);

            await _localStorage.SetItemAsync("cart", cart);

            OnChange.Invoke();
        }
    }
}
