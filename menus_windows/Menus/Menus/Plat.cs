using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace Menus
{
    public class Plat : IValueConverter
    {
        public long id;
        public string nom;
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

        public int temps { get; set; }

        public object Convert(object value, System.Type targetType, object parameter, string language)
        {
            Debug.WriteLine(value);
            return TimeSpan.FromSeconds((int)value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
