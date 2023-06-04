using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Linq;

namespace TelnetKingdoms.Data
{
    static public class DataCore
    {
        private class TrackedDataLibrary : Dictionary<int, object>
        {
            public System.Type ValueType { get; set; }
        }

        static private DataSet _dataSet;
        static private Dictionary<System.Type, TrackedDataLibrary> _tracked_tables;

        #region static constructor

        static DataCore()
        {
            // data set to hold all data
            _dataSet = new DataSet();

            // cached instances of tables
            _tracked_tables = new Dictionary<Type, TrackedDataLibrary>();

            // find all classes that implement DataTrackedObject
            var trackableObjects = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(DataTrackedObject).IsAssignableFrom(x));

            // create a table in _dataSet, and a cache instance in _tracked_tables
            foreach (var trackableObject in trackableObjects)
            {
                // ignore abstrack objects
                if (trackableObject.IsAbstract)
                    continue;

                // construct the table in the data set
                DataTable table = new DataTable(trackableObject.Name);
                table.Columns.AddRange(GetDataColumns(trackableObject).ToArray());
                _dataSet.Tables.Add(table);

                // add a cached collection to the tracked tables
                TrackedDataLibrary tdl = new TrackedDataLibrary() { ValueType = trackableObject };
                _tracked_tables.Add(trackableObject, tdl);
            }
        }

        static private void InitializeContent()
        {
            Console.WriteLine("DataCore: Empty database.  Loading content.");

            // you really should mantain the loading order here
            NaturalResourceTemplate.LoadFromJson("natural_resources.json");
            Title.LoadFromJson("fiefs.json");
            Fief.LoadFromJson("fiefs.json");
            // TODO: load characters

            Console.WriteLine("DataCore: Content loading complete.");
        }

        #endregion

        #region public methods

        static public T CreateNewInstance<T>() where T : DataTrackedObject
        {
            // add to database
            DataTable table = _dataSet.Tables[typeof(T).Name];
            DataRow row = table.NewRow();
            table.Rows.Add(row);

            // add to cache
            var inst = (T)Activator.CreateInstance(typeof(T), row);
            _tracked_tables[typeof(T)].Add(inst.UID, inst);

            return inst;
        }

        static public void DeleteInstance(DataTrackedObject instance)
        {
            // error checking
            if (!_tracked_tables[instance.GetType()].ContainsKey(instance.UID))
                return;

            // remove from cache
            _tracked_tables[instance.GetType()].Remove(instance.UID);

            // remove from database
            DataTable table = _dataSet.Tables[instance.GetType().Name];
            table.Rows.Remove(instance.DataRow);
        }

        static public IEnumerable<T> GetAll<T>() where T : DataTrackedObject
        {
            return _tracked_tables[typeof(T)].Values.Cast<T>();
        }

        static public T? Get<T>(int uid, T? defaultValue) where T : DataTrackedObject
        {
            if (!_tracked_tables[typeof(T)].TryGetValue(uid, out var item))
                return defaultValue;

            return (T)Convert.ChangeType(item, typeof(T));
        }

        static public IEnumerable<T> Query<T>(System.Func<T, bool> query) where T : DataTrackedObject
        {
            return _tracked_tables[typeof(T)]
                .Values
                .Where(x => query((T)x))
                .Cast<T>();
        }

        static public void Serialize(string fileName)
        {
            _dataSet.WriteXmlSchema(fileName + ".schema");
            _dataSet.WriteXml(fileName);
        }

        static public void Deserialize(string fileName)
        {
            if (!File.Exists(fileName))
            {
                InitializeContent();
                return;
            }

            DataSet set = new DataSet();
            set.ReadXmlSchema(fileName + ".schema");
            set.ReadXml(fileName);

            // accept new data set
            _dataSet = set;

            // refresh cache
            foreach (TrackedDataLibrary tdl in _tracked_tables.Values)
            {
                tdl.Clear();

                foreach (DataRow row in _dataSet.Tables[tdl.ValueType.Name].Rows)
                {
                    DataTrackedObject trackedObject = (DataTrackedObject) Activator.CreateInstance(tdl.ValueType, row);
                    tdl.Add(trackedObject.UID, trackedObject);
                }
            }

            Console.WriteLine("DataCore: Finished loading content.");
        }

        static public List<DataColumn> GetDataColumns(Type t)
        {
            var list = new List<DataColumn>();

            DataColumn column = new DataColumn();
            column.ColumnName = "UID";
            column.AutoIncrement = true;
            column.DataType = typeof(int);
            column.Unique = true;
            column.ReadOnly = true;
            list.Add(column);

            foreach (var prop in t.GetProperties())
            {
                bool isDataTracked = prop.GetCustomAttribute<DataTrackedColumnAttribute>() != null;
                bool isDataTrackedReference = prop.PropertyType.IsGenericType ? prop.PropertyType.GetGenericTypeDefinition() == typeof(DataTrackedReference<>) : false;

                // accept tracked properties
                if (prop.GetCustomAttribute<DataTrackedColumnAttribute>() != null)
                {
                    column = new DataColumn();
                    column.ColumnName = prop.Name;
                    column.AutoIncrement = false;
                    column.DataType = prop.PropertyType;
                    column.Unique = false;
                    column.ReadOnly = false;
                    list.Add(column);
                }
                else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DataTrackedReference<>))
                {
                    column = new DataColumn();
                    column.ColumnName = prop.Name;
                    column.AutoIncrement = false;
                    column.DataType = typeof(int);
                    column.Unique = false;
                    column.ReadOnly = false;
                    list.Add(column);
                }
            }

            return list;
        }

        #endregion
    }
}
