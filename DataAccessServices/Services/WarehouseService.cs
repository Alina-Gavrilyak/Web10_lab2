using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices.Services
{
    public class WarehouseService
    {
        private readonly TurnoverRepository repository;
        private static int _nextId = 1;

        public WarehouseService(TurnoverRepository repository)
        {
            this.repository = repository;
            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            if (repository.Warehouses.Count > 0)
                return;
            Add(new Warehouse { Number = 1, Address = "Groceries" });
            Add(new Warehouse { Number = 2, Address = "Toys" });
            Add(new Warehouse { Number = 3, Address = "Hardware" });

        }

        public IEnumerable<Warehouse> GetAll()
        {
            return repository.Warehouses;
        }

        public Warehouse Get(int id)
        {
            return repository.Warehouses.Find(w => w.Id == id);
        }

        public Warehouse Add(Warehouse item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.Id = _nextId++;
            repository.Warehouses.Add(item);
            return item;
        }

        public void Remove(int id)
        {
            repository.Warehouses.RemoveAll(w => w.Id == id);
        }


        public bool Update(Warehouse item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = repository.Warehouses.FindIndex(w => w.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            repository.Warehouses.RemoveAt(index);
            repository.Warehouses.Add(item);

            return true;
        }
    }
}
