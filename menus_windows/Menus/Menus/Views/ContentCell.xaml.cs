using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Menus
{
    public sealed partial class ContentCell : UserControl
    {
        ObservableCollection<Plat> listPlats;
        DatabaseHandler databaseHandler;
        Plat plat;

        public ObservableCollection<Plat> ListPlats
        {
            get { return listPlats; }
            set
            {
                listPlats = value;
                comboBox.ItemsSource = listPlats;
            }
        }

        // Date coresponding to the colum this cell is in
        public DateTime Date { get; set; }

        public Plat Plat
        {
            get { return plat; }
            set
            {
                plat = value;
                comboBox.SelectedItem = value;
                platName.Text = plat != null ? plat.nom : string.Empty;
            }
        }

        public ContentCell()
        {
            this.InitializeComponent();
            databaseHandler = DatabaseHandler.Instance;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int column = Grid.GetColumn(this);
            int row = Grid.GetRow(this);
            Grid parent = (Grid)this.Parent;
            double simpleBorder = Semaine.simpleBorder;
            double doubleBorder = Semaine.doubleBorder;

            double left = column == 0 ? doubleBorder : simpleBorder;
            double top = simpleBorder;
            double right = column == parent.ColumnDefinitions.Count - 1 ? doubleBorder : simpleBorder;
            double bottom = row == parent.RowDefinitions.Count - 1 ? doubleBorder : simpleBorder;

            border.BorderThickness = new Thickness(left, top, right, bottom);
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            relativePanel.Opacity = 1;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!comboBox.IsDropDownOpen)
                relativePanel.Opacity = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            if (box.SelectedItem is Plat plat)
            {
                if (databaseHandler.InsertPlatInSemaines(plat, Date))
                {
                    this.Plat = plat;
                    platName.Text = plat.nom;
                }
                else
                {
                    box.SelectedItem = this.Plat;
                    box.SelectedIndex = box.Items.IndexOf(this.Plat);
                }
            }
            else
            {
                this.Plat = null;
                platName.Text = string.Empty;
            }
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            comboBox.SelectedIndex = -1;
            comboBox.SelectedItem = null;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            relativePanel.MaxWidth = e.NewSize.Width;
        }
    }
}
