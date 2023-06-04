using Newtonsoft.Json;
using System.Data;
using System.Reflection.PortableExecutable;

namespace TelnetKingdoms.Data
{
    public class Fief : DataTrackedObject
    {
        [DataTrackedColumn] public string Name { get => GetColumnValue<string>(nameof(Name)); set => SetColumnValue(nameof(Name), value); }
        [DataTrackedColumn] public int Population { get => GetColumnValue<int>(nameof(Population)); set => SetColumnValue(nameof(Population), value); }
        public DataTrackedReference<Title> Investiture { get; init; } // the title that grants ownership over this fief
        public DataTrackedReference<Container> InventoryContainer { get; init; }

        //public IEnumerable<Building> Buildings { get => DataCore.Query<Building>(x => x.InFief != null && x.InFief.UID == this.UID); }
        public IEnumerable<NaturalResourceInstance> NaturalResources { get => DataCore.Query<NaturalResourceInstance>(x => x.InFief != null && x.InFief.UID == this.UID); }
        public Character Owner { get => Investiture.Value.TitleHolder.Value; }

        public Fief(DataRow row) : base(row)
        {
            Investiture = new DataTrackedReference<Title>(this, nameof(Investiture));
            InventoryContainer = new DataTrackedReference<Container>(this, nameof(InventoryContainer));
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
                var natural_resouce_templates = DataCore.GetAll<NaturalResourceTemplate>();
                Random rand = new Random();

                // create fiefs
                foreach (var entry_data in data)
                {
                    var entry = DataCore.CreateNewInstance<Fief>();
                    entry.Name = entry_data.name;
                    entry.Population = entry_data.population;
                    entry.InventoryContainer.Value = DataCore.CreateNewInstance<Container>();

                    // create natural resources
                    int num_recs = rand.Next(2, 4); // 2 or 3 resources per fief
                    var open_recs = natural_resouce_templates.ToList();
                    for (int i = 0; i < num_recs; i++)
                    {
                        // pick from the weighted list
                        float max = open_recs.Sum(x => x.Rarity);
                        float roll = rand.NextSingle() * max;
                        var rec_temp = open_recs.FirstOrDefault(x => roll >= (max -= x.Rarity));

                        // default if something weird happened
                        if (rec_temp == null)
                            rec_temp = open_recs[0];

                        // remove chosen from open list
                        open_recs.Remove(rec_temp);

                        // create a resource instance
                        var rec = DataCore.CreateNewInstance<NaturalResourceInstance>();
                        rec.Template.Value = rec_temp;
                        rec.GrowthRate = rand.Next(rec_temp.MinGrowthRate, rec_temp.MaxGrowthRate);
                        rec.Access = rand.NextSingle() * (rec_temp.MaxAccess - rec_temp.MinAccess) + rec_temp.MinAccess;
                        rec.InFief.Value = entry;
                    }

                    // TODO: create buildings

                    // assign title
                    entry.Investiture.Value = DataCore.Query<Title>(x => x.Name == entry_data.title).FirstOrDefault();
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
