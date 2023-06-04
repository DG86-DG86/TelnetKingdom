using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class InventoryStack : DataTrackedObject
    {
        public DataTrackedReference<Product> Product { get; init; }
        [DataTrackedColumn] public int Count { get; set; }
        public DataTrackedReference<Container> InContainer { get; init; }

        public InventoryStack(DataRow dataRow) : base(dataRow)
        {
            Product = new DataTrackedReference<Product>(this, nameof(Product));
            InContainer = new DataTrackedReference<Container>(this, nameof(InContainer));
        }
    }
}
