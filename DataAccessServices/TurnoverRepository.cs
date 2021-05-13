using DataAccessContracts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessServices
{
    public class TurnoverRepository
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Shop> Shops { get; set; } = new List<Shop>();
        public List<ProductShop> ProductShops { get; set; } = new List<ProductShop>();
        public List<ProductWarehouse> ProductWarehouses { get; set; } = new List<ProductWarehouse>();
        public List<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public List<RequestDelivery> RequestDeliverys { get; set; } = new List<RequestDelivery>();
        public List<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();
    }
}
