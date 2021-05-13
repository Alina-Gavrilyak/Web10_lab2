using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class ProductService
    {
        private static int _nextId = 1;

        private readonly TurnoverRepository repository;

        public ProductService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.Products.Count > 0)
                return;
            Add(new Product { Number = 1, Name = "Tomato soup", Category = "Groceries", Price = 1.39M });
            Add(new Product { Number = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M });
            Add(new Product { Number = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M });
        }

        public IEnumerable<Product> GetAll()
        {
            return repository.Products;
        }

        public Product Get(int id)
        {
            return repository.Products.Find(p => p.Id == id);
        }

        public Product Add(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Id = _nextId++;
            repository.Products.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.Products.RemoveAll(p => p.Id == id);
        }


        public bool Update(Product item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.Products.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.Products.RemoveAt(index);
            repository.Products.Add(item);

            return true;
        }

    }
}
