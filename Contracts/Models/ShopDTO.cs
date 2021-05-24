using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Models {
    public class ShopBaseDTO {
        public string Address { get; set; }
    }

    public class ShopInputDTO : ShopBaseDTO {
        public List<int> ProductIds { get; set; }
    }

    public class ShopDTO : ShopBaseDTO {
        public int Id { get; set; }
        public virtual List<ProductDTO> Products { get; set; }
        public virtual List<RequestDeliveryDTO> RequestDeliveries { get; set; }
    }
}
