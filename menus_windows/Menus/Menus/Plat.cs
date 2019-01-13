using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menus
{
    public class Plat
    {
        public long id;
        public string nom;
        public string type;
        public string saison;
        public TimeSpan temps;
        public int note;
        public string ingredients;
        public string description;

        public Plat(long id, string nom, string type, string saison, TimeSpan temps, int note, string ingredients, string description)
        {
            this.id = id;
            this.nom = nom;
            this.type = type;
            this.saison = saison;
            this.temps = temps;
            this.note = note;
            this.ingredients = ingredients;
            this.description = description;
        }

        public override string ToString()
        {
            return nom;
        }
    }
}
