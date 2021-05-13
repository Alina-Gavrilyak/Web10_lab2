using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class ProductWarehouseService
    {
        private static int _nextId = 1;

        private readonly TurnoverRepository repository;

        public ProductWarehouseService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.ProductWarehouses.Count > 0)
                return;
            Add(new ProductWarehouse { ProductId = 1, WarehouseId= 1 });
            Add(new ProductWarehouse { ProductId = 2, WarehouseId = 3 });
            Add(new ProductWarehouse { ProductId = 3, WarehouseId = 2 });
        }

        public IEnumerable<ProductWarehouse> GetAll()
        {
            return repository.ProductWarehouses;
        }

        public ProductWarehouse Get(int id)
        {
            return repository.ProductWarehouses.Find(pw => pw.Id == id);
        }

        public ProductWarehouse Add(ProductWarehouse item)
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

            var warehous = repository.Warehouses.Find(w => w.Id == item.WarehouseId);
            if (warehous == null)
                item.WarehouseId = 0;
            else
                item.Warehouse = warehous;

            repository.ProductWarehouses.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.ProductWarehouses.RemoveAll(pw => pw.Id == id);
        }


        public bool Update(ProductWarehouse item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.ProductWarehouses.FindIndex(pw => pw.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.ProductWarehouses.RemoveAt(index);

            var product = repository.Products.Find(p => p.Id == item.ProductId);
            if (product == null)
                item.ProductId = 0;
            else
                item.Product = product;

            var warehous = repository.Warehouses.Find(w => w.Id == item.WarehouseId);
            if (warehous == null)
                item.WarehouseId = 0;
            else
                item.Warehouse = warehous;

            repository.ProductWarehouses.Add(item);

            return true;
        }
    }
}
