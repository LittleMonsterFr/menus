using Menus.Classes;
using System;
using Windows.UI.Xaml.Data;

namespace Menus
{
    public class Plat : BindableBase, IValueConverter
    {
        private string _nom;

        public long id;
        public Type type;
        public Saison saison;
        public int note;
        public string ingredients;
        public string description;

        public Plat()
        {
        }

        public Plat(long id, string nom, Type type, Saison saison, int temps, int note, string ingredients, string description)
        {
            this.id = id;
            this._nom = nom;
            this.type = type;
            this.saison = saison;
            this.Temps = temps;
            this.note = note;
            this.ingredients = ingredients;
            this.description = description;
        }

        public string Nom
        {
            get { return this._nom; }
            set { this.SetProperty(ref this._nom, value); }
        }

        public override string ToString()
        {
            return Nom;
        }

        public int Temps { get; set; }

        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((int)value);
            return timeSpan.Hours + "h " + timeSpan.Minutes + "m";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is Plat plat &&
                   id == plat.id &&
                   Nom == plat.Nom &&
                   type.Equals(plat.type) &&
                   saison.Equals(plat.saison) &&
                   note == plat.note &&
                   ingredients.Equals(plat.ingredients) &&
                   description.Equals(plat.description) &&
                   Temps == plat.Temps;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, Nom, type, saison, note, ingredients, description, Temps);
        }
    }
}
