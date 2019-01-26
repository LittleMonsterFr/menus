using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menus
{
    public class Saison
    {
        public Saison(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            var saison = obj as Saison;
            return saison != null &&
                   Id == saison.Id &&
                   Name.Equals(saison.Name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public override string ToString()
        {
            return Name;
        }


    }
}
