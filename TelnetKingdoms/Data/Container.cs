using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class Container : DataTrackedObject
    {
        public IEnumerable<InventoryStack> Contents => DataCore.Query<InventoryStack>(x => x.InContainer.UID == this.UID);

        public Container(DataRow dataRow) : base(dataRow) { }
    }
}
