using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessContracts.Entities
{
    public class DeliveryItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int RequestDeliveryId { get; set; }
        public virtual RequestDelivery RequestDelivery { get; set; }
    }
}
