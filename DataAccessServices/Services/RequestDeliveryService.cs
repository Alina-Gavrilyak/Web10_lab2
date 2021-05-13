using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class RequestDeliveryService
    {
        private readonly TurnoverRepository repository;
        private static int _nextId = 1;

        public RequestDeliveryService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.RequestDeliverys.Count > 0)
                return;
            Add(new RequestDelivery { Number = 1, WarehouseId = 3 });
            Add(new RequestDelivery { Number = 2, WarehouseId = 1 });
            Add(new RequestDelivery { Number = 3, ShopId = 3 });

        }

        public IEnumerable<RequestDelivery> GetAll()
        {
            return repository.RequestDeliverys;
        }

        public RequestDelivery Get(int id)
        {
            return repository.RequestDeliverys.Find(r => r.Id == id);
        }

        public RequestDelivery Add(RequestDelivery item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Id = _nextId++;

            var warehouse = repository.Warehouses.Find(w => w.Id == item.WarehouseId);
            if (warehouse == null)
                item.WarehouseId = 0;
            else
                item.Warehouse = warehouse;

            var shop = repository.Shops.Find(s => s.Id == item.ShopId);
            if (shop == null)
                item.ShopId = 0;
            else
                item.Shop = shop;

            repository.RequestDeliverys.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.RequestDeliverys.RemoveAll(r => r.Id == id);
        }


        public bool Update(RequestDelivery item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.RequestDeliverys.FindIndex(r => r.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.RequestDeliverys.RemoveAt(index);
            
            var warehouse = repository.Warehouses.Find(w => w.Id == item.WarehouseId);
            if (warehouse == null)
                item.WarehouseId = 0;
            else
                item.Warehouse = warehouse;

            var shop = repository.Shops.Find(s => s.Id == item.ShopId);
            if (shop == null)
                item.ShopId = 0;
            else
                item.Shop = shop;

            repository.RequestDeliverys.Add(item);

            return true;
        }
    }
}
