using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class DeliveryItemService
    {
        private static int _nextId = 1;

        private readonly TurnoverRepository repository;

        public DeliveryItemService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.DeliveryItems.Count > 0)
                return;
            Add(new DeliveryItem { ProductId = 1, RequestDeliveryId = 2 });
            Add(new DeliveryItem { ProductId = 2, RequestDeliveryId = 1 });
            Add(new DeliveryItem { ProductId = 3, RequestDeliveryId = 3 });
        }

        public IEnumerable<DeliveryItem> GetAll()
        {
            return repository.DeliveryItems;
        }

        public DeliveryItem Get(int id)
        {
            return repository.DeliveryItems.Find(d => d.Id == id);
        }

        public DeliveryItem Add(DeliveryItem item)
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

            var requestDelivery = repository.RequestDeliverys.Find(r => r.Id == item.RequestDeliveryId);
            if (requestDelivery == null)
                item.RequestDeliveryId = 0;
            else
                item.RequestDelivery = requestDelivery;

            repository.DeliveryItems.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.DeliveryItems.RemoveAll(d => d.Id == id);
        }


        public bool Update(DeliveryItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.DeliveryItems.FindIndex(d => d.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.DeliveryItems.RemoveAt(index);

            var product = repository.Products.Find(p => p.Id == item.ProductId);
            if (product == null)
                item.ProductId = 0;
            else
                item.Product = product;

            var requestDelivery = repository.RequestDeliverys.Find(r => r.Id == item.RequestDeliveryId);
            if (requestDelivery == null)
                item.RequestDeliveryId = 0;
            else
                item.RequestDelivery = requestDelivery;

            repository.DeliveryItems.Add(item);

            return true;
        }
    }
}
