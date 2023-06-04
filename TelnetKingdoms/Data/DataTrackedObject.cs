using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TelnetKingdoms.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DataTrackedColumnAttribute : Attribute { }

    public class DataTrackedColumn<T>
    {
        private DataTrackedObject parent;

        public string ColumnName { get; init; }

        public T Value { get => parent.GetColumnValue<T>(ColumnName); set => parent.SetColumnValue(ColumnName, value); }

        public DataTrackedColumn(DataTrackedObject parent, string name)
        {
            this.parent = parent;
            ColumnName = name;
        }
    }

    public abstract class DataTrackedObject
    {
        public DataRow DataRow { get; init; }
        public int UID { get => GetColumnValue<int>("UID"); init => SetColumnValue("UID", value); }

        public DataTrackedObject(DataRow dataRow) => DataRow = dataRow;

        public T GetColumnValue<T>(string columnName) => (T)Convert.ChangeType(DataRow[columnName], typeof(T));
        public void SetColumnValue(string columnName, object value) => DataRow[columnName] = value;
    }
}
