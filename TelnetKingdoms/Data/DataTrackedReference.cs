using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class DataTrackedReference<T> where T : DataTrackedObject
    {
        private DataRow _row;
        private string _name;

        public int UID
        {
            get => (int) _row[_name];
            set => _row[_name] = value;
        }

        public string ColumnName { get => _name; }

        public T? Value
        {
            get => DataCore.Get<T>(UID, null);
            set => UID = value != null ? value.UID : default(int);
        }

        public DataTrackedReference(DataTrackedObject parent, string name)
        {
            _row = parent.DataRow;
            _name = name;
        }
    }
}
