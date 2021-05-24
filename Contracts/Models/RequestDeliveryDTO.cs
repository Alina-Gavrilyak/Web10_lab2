using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Models {
    public class RequestDeliveryBaseDTO {
        public int Number { get; set; }
        public int WarehouseId { get; set; }
        public int ShopId { get; set; }
    }

    public class RequestDeliveryInputDTO : RequestDeliveryBaseDTO {
        public List<int> ProductIds { get; set; }
    }

    public class RequestDeliveryDTO : RequestDeliveryBaseDTO {
        public int Id { get; set; }
        public virtual WarehouseDTO Warehouse { get; set; }
        public virtual ShopDTO Shop { get; set; }
        public virtual List<ProductDTO> Products { get; set; }
    }
}
