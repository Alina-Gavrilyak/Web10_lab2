using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessContracts.Entities
{
    public class ProductWarehouse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }

    }
}
