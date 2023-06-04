using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TelnetKingdoms.Data
{
    public class NaturalResourceTemplate : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get => GetColumnValue<string>(nameof(Name)); set => SetColumnValue(nameof(Name), value); }
        [DataTrackedColumn] public string Description { get => GetColumnValue<string>(nameof(Description)); set => SetColumnValue(nameof(Description), value); }
        [DataTrackedColumn] public float Rarity { get => GetColumnValue<float>(nameof(Rarity)); set => SetColumnValue(nameof(Rarity), value); } // number from 0.0 to 1.0, where 1.0 is very common
        [DataTrackedColumn] public int MinGrowthRate { get => GetColumnValue<int>(nameof(MinGrowthRate)); set => SetColumnValue(nameof(MinGrowthRate), value); } // smallest randomized growth rate
        [DataTrackedColumn] public int MaxGrowthRate { get => GetColumnValue<int>(nameof(MaxGrowthRate)); set => SetColumnValue(nameof(MaxGrowthRate), value); } // largest randomized growth rate
        [DataTrackedColumn] public float MinAccess { get => GetColumnValue<float>(nameof(MinAccess)); set => SetColumnValue(nameof(MinAccess), value); } // smallest randomized access
        [DataTrackedColumn] public float MaxAccess { get => GetColumnValue<float>(nameof(MaxAccess)); set => SetColumnValue(nameof(MaxAccess), value); } // largest randomized access

        public NaturalResourceTemplate(DataRow dataRow) : base(dataRow) { }

        static public void LoadFromJson(string filepath)
        {
            var template = new {
                Name = string.Empty,
                Description = string.Empty,
                Rarity = 0f,
                MinGrowthRate = 0,
                MaxGrowthRate = 0,
                MinAccess = 0f,
                MaxAccess = 0f,
                };

            var template_list = Enumerable.Range(0,0).Select(x => template).ToList();

            try
            {
                var data = JsonConvert.DeserializeAnonymousType(File.ReadAllText(filepath), template_list);

                foreach (var entry_data in data)
                {
                    var entry = DataCore.CreateNewInstance<NaturalResourceTemplate>();
                    entry.Name = entry_data.Name;
                    entry.Description = entry_data.Description;
                    entry.Rarity = entry_data.Rarity;
                    entry.MinGrowthRate = entry_data.MinGrowthRate;
                    entry.MaxGrowthRate = entry_data.MaxGrowthRate;
                    entry.MinAccess = entry_data.MinAccess;
                    entry.MaxAccess = entry_data.MaxAccess;
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }

    public class NaturalResourceInstance : DataTrackedObject
    {
        public DataTrackedReference<NaturalResourceTemplate> Template { get; set; }
        public DataTrackedReference<Fief> InFief { get; set; }
        [DataTrackedColumn] public int Volume { get => GetColumnValue<int>(nameof(Volume)); set => SetColumnValue(nameof(Volume), value); } // current number of available units
        [DataTrackedColumn] public int GrowthRate { get => GetColumnValue<int>(nameof(GrowthRate)); set => SetColumnValue(nameof(GrowthRate), value); } // how much volume grows per turn
        [DataTrackedColumn] public float Access { get => GetColumnValue<float>(nameof(Access)); set => SetColumnValue(nameof(Access), value); } // value from 0.0 to 1.0 that modifies how easy it is to extract volume

        public NaturalResourceInstance(DataRow dataRow) : base(dataRow)
        {
            Template = new DataTrackedReference<NaturalResourceTemplate>(this, nameof(Template));
            InFief = new DataTrackedReference<Fief>(this, nameof(InFief));
        }
    }
}
