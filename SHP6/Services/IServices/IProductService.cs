using SHP6.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SHP6.Services
{
    public interface IProductService 
    {
        List<Product> SortBy(string sortBy);
        IEnumerable<Product> GetCartGuest();
        IEnumerable<Product> GetCartUser();
        Product GetProductById(int id);

        void AddProducts(Product product);
        void AddProductToCart(Product product);
        void ScanStore();
        void Checkout();
        void RemoveFromCart(int id);
    }
}
