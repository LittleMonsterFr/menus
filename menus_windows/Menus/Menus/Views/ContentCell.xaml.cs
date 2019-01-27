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
        Plat plat;
        ObservableCollection<Plat> listPlats;
        DatabaseHandler databaseHandler;
        DateTime date;

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
        public DateTime Date {
            get { return date; }
            set
            {
                date = value;
                long plat_id = databaseHandler.GetPlatIdForDateByTypeId(date, Grid.GetRow(this));
                if (plat_id != 0)
                {
                    foreach (Plat plat in listPlats)
                    {
                        if (plat.id == plat_id)
                        {
                            comboBox.SelectedItem = plat;
                            platName.Text = plat.nom;
                            break;
                        }
                    }
                }
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
                    this.plat = plat;
                    platName.Text = plat.nom;
                }
                else
                {
                    box.SelectedItem = this.plat;
                    box.SelectedIndex = box.Items.IndexOf(this.plat);
                }
            }
            else
            {
                this.plat = null;
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
