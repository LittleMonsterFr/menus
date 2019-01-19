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
        public long type;
        public long saison;
        public int temps;
        public int note;
        public string ingredients;
        public string description;

        public Plat(long id, string nom, long type, long saison, int temps, int note, string ingredients, string description)
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
