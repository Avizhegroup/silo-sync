using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silo.Ui.Gate
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationTitle { get; set; }
        public DestinationOperationalType OperationalType { get; set; }
        public DestinationInventoryType InventoryType { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
    public enum DestinationOperationalType
    {
        NotSpecified = -1,
        Production = 1,
        Product = 2,
        Material = 4,
        Waste = 5,
        Loading = 3
    }

    public enum DestinationInventoryType
    {
        NotSpecified = -1,
        Virtual,
        Physical
    }
}
