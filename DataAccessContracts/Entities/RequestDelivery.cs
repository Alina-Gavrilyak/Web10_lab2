using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessContracts.Entities
{
    public class RequestDelivery
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }
        public int ShopId { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
