using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class Character : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get => GetColumnValue<string>(nameof(Name)); set => SetColumnValue(nameof(Name), value); }

        public Character(DataRow dataRow) : base(dataRow)
        {
        }
    }
}
