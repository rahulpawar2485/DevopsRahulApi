using InMemoryCrudApi.Models;

namespace InMemoryCrudApi.Repository
{
    public static class ProductRepository
    {
        private static readonly List<Product> _products = new();
        private static int _id = 1;

        public static List<Product> GetAll() => _products;

        public static Product GetById(int id) =>
            _products.FirstOrDefault(p => p.Id == id);

        public static Product Add(Product product)
        {
            product.Id = _id++;
            _products.Add(product);
            return product;
        }

        public static bool Update(int id, Product product)
        {
            var existing = GetById(id);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.Price = product.Price;
            return true;
        }

        public static bool Delete(int id)
        {
            var product = GetById(id);
            if (product == null) return false;

            _products.Remove(product);
            return true;
        }
    }
}
