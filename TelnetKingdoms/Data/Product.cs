using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class Product : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get; set; }
        [DataTrackedColumn] public string Description { get; set; }
        [DataTrackedColumn] public float Value { get; set; }

        public Product(DataRow row) : base(row) { }
    }

}
