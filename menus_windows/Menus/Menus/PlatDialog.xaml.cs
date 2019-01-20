using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    public sealed partial class PlatDialog : ContentDialog
    {
        private DatabaseHandler databaseHandler;
        private ObservableCollection<Type> types;
        private ObservableCollection<Saison> saisons;
        private Dictionary<long, ObservableCollection<Plat>> lists;

        public PlatDialog(DatabaseHandler databaseHandler, Dictionary<long, ObservableCollection<Plat>> lists)
        {
            this.InitializeComponent();
            this.databaseHandler = databaseHandler;
            this.lists = lists;
            types = new ObservableCollection<Type>(databaseHandler.GetTypes());
            saisons = new ObservableCollection<Saison>(databaseHandler.GetSaisons());
        }

        private bool ValidatePlatInput()
        {
            bool res = true;
            SolidColorBrush red = new SolidColorBrush(Windows.UI.Colors.Red);

            if (string.IsNullOrEmpty(nom.Text))
            {
                nom.BorderBrush = red;
                res = false;
            }
            else
                nom.ClearValue(BorderBrushProperty);

            if (type.SelectedValue == null)
            {
                type.BorderBrush = red;
                res = false;
            }
            else
                type.ClearValue(BorderBrushProperty);

            if (saison.SelectedValue == null)
            {
                saison.BorderBrush = red;
                res = false;
            }
            else
                saison.ClearValue(BorderBrushProperty);

            return !res;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = ValidatePlatInput();
            if (args.Cancel == false)
            {
                Plat plat = new Plat(0, nom.Text, (Type)type.SelectedValue, (Saison)saison.SelectedValue, (int)temps.Time.TotalSeconds, (int)note.Value, ingredients.Text, description.Text);
                databaseHandler.InsertPlat(plat);
                lists[plat.type.Id].Add(plat);
            }
            deferral.Complete();
        }
    }
}
