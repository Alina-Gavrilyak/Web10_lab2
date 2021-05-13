using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class ShopService
    {
        private readonly TurnoverRepository repository;
        private static int _nextId = 1;

        public ShopService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.Shops.Count > 0)
                return;
            Add(new Shop { Address = "Prospekt Pobedy 12" });
            Add(new Shop { Address = "Prospekt Kosmonavta 5" });
            Add(new Shop { Address = "Prospekt Mira 8" });

        }

        public IEnumerable<Shop> GetAll()
        {
            return repository.Shops;
        }

        public Shop Get(int id)
        {
            return repository.Shops.Find(s => s.Id == id);
        }

        public Shop Add(Shop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Id = _nextId++;
            repository.Shops.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.Shops.RemoveAll(s => s.Id == id);
        }


        public bool Update(Shop item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.Shops.FindIndex(s => s.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.Shops.RemoveAt(index);
            repository.Shops.Add(item);

            return true;
        }
    }
}
