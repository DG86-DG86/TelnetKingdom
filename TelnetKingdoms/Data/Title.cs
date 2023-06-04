using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace TelnetKingdoms.Data
{
    public class Title : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get => GetColumnValue<string>(nameof(Name)); set => SetColumnValue(nameof(Name), value); }
        [DataTrackedColumn] public int Level { get => GetColumnValue<int>(nameof(Level)); set => SetColumnValue(nameof(Level), value); }
        public DataTrackedReference<Title> Liege; // the title that holds dominion over this title and its lands
        public DataTrackedReference<Character> TitleHolder; // the character that currently holds this title

        public IEnumerable<Title> Vassals { get => DataCore.Query<Title>(x => x.Liege.UID == this.UID); } // a list of titles that call this title liege

        public Title(DataRow dataRow) : base(dataRow)
        {
            Liege = new DataTrackedReference<Title>(this, nameof(Liege));
            TitleHolder = new DataTrackedReference<Character>(this, nameof(TitleHolder));
        }

        static public void LoadFromJson(string filepath)
        {
            var template = new
            {
                name = string.Empty,
                description = string.Empty,
                population = 0,
                nationality = string.Empty,
                tenantInChief = string.Empty,
                title = string.Empty,
            };

            var template_list = Enumerable.Range(0, 0).Select(x => template).ToList();

            try
            {
                var data = JsonConvert.DeserializeAnonymousType(File.ReadAllText(filepath), template_list);

                // create titles
                List<string> closed_title_list = new List<string>();
                int french_level = data.Count(x => x.nationality == "French");
                int english_level = data.Count(x => x.nationality == "English");
                foreach (var entry_data in data.OrderByDescending(x => x.population)) // titles get ranks according to population
                {
                    // only allow one instance of each title
                    if (closed_title_list.Contains(entry_data.title))
                        continue;

                    var entry = DataCore.CreateNewInstance<Title>();
                    entry.Name = entry_data.title;
                    closed_title_list.Add(entry_data.title);

                    // set Level
                    if (entry_data.nationality == "French")
                    {
                        entry.Level = french_level;
                        french_level--;
                    }
                    else
                    {
                        entry.Level = english_level;
                        english_level--;
                    }

                    // TODO: set Liege
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
