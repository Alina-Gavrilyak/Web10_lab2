using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class ProductShopService
    {
        private static int _nextId = 1;

        private readonly TurnoverRepository repository;

        public ProductShopService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.ProductShops.Count > 0)
                return;
            Add(new ProductShop { ProductId = 1, ShopId = 1 });
            Add(new ProductShop { ProductId = 2, ShopId = 3 });
            Add(new ProductShop { ProductId = 3, ShopId = 2 });
        }

        public IEnumerable<ProductShop> GetAll()
        {
            return repository.ProductShops;
        }

        public ProductShop Get(int id)
        {
            return repository.ProductShops.Find(ps => ps.Id == id);
        }

        public ProductShop Add(ProductShop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Id = _nextId++;

            var product = repository.Products.Find(p => p.Id == item.ProductId);
            if (product == null)
                item.ProductId = 0;
            else
                item.Product = product;

            var shop = repository.Shops.Find(s => s.Id == item.ShopId);
            if (shop == null)
                item.ShopId = 0;
            else
                item.Shop = shop;

            repository.ProductShops.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.ProductShops.RemoveAll(ps => ps.Id == id);
        }


        public bool Update(ProductShop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.ProductShops.FindIndex(ps => ps.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.ProductShops.RemoveAt(index);

            var product = repository.Products.Find(p => p.Id == item.ProductId);
            if (product == null)
                item.ProductId = 0;
            else
                item.Product = product;

            var shop = repository.Shops.Find(s => s.Id == item.ShopId);
            if (shop == null)
                item.ShopId = 0;
            else
                item.Shop = shop;

            repository.ProductShops.Add(item);

            return true;
        }
    }
}
