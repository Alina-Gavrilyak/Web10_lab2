using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Models {
    public class WarehouseBaseDTO {
        public int Number { get; set; }
        public string Address { get; set; }
    }

    public class WarehouseInputDTO : WarehouseBaseDTO {
        public List<int> ProductIds { get; set; }
    }

    public class WarehouseDTO : WarehouseBaseDTO {
        public int Id { get; set; }
        public virtual List<ProductDTO> Products { get; set; }
        public virtual List<RequestDeliveryDTO> RequestDeliveries { get; set; }
    }
}
