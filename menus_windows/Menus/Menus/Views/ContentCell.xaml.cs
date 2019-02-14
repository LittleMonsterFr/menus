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
        // Private reference to the corresponding list of plat (starter, main, dinner)
        private ObservableCollection<Plat> _listPlats;

        // Database handler use to execute database operations
        private DatabaseHandler databaseHandler;

        // Private reference to the plat assigned to the cell
        private Plat _plat;

        // Indicate if the comboxbox handler skips the update or not
        private bool updateComboBox = true;

        // Public accesser to the list of plats
        public ObservableCollection<Plat> ListPlats
        {
            get { return _listPlats; }
            set
            {
                _listPlats = value;
                comboBox.ItemsSource = _listPlats;
            }
        }

        // Public reference coresponding to the colum this cell is in
        public DateTime Date { get; set; }

        // Public reference to the plat selected in the cell used when the semaine is loaded
        public Plat Plat
        {
            get { return _plat; }
            set
            {
                _plat = value;
                updateComboBox = false;
                comboBox.SelectedItem = _plat;
                comboBox.SelectedIndex = comboBox.Items.IndexOf(_plat);
                updateComboBox = true;
                platName.Text = _plat != null ? _plat.nom : string.Empty;
            }
        }

        // Default constructor
        public ContentCell()
        {
            this.InitializeComponent();

            // Get the database instance
            databaseHandler = DatabaseHandler.Instance;
        }

        // Defines the border at loading time
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
            // Display the panel containing the combobox when the mouse is on this cell
            platSelectionPanel.Opacity = 1;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Hide the panel containing the combobox when the mouse leaves the cell and the dropdown is closed
            if (!comboBox.IsDropDownOpen)
                platSelectionPanel.Opacity = 0;
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (updateComboBox)
            {
                ComboBox box = sender as ComboBox;

                if (box.SelectedItem is Plat plat)
                {
                    if (await databaseHandler.InsertPlatInSemaines(plat, Date))
                    {
                        _plat = plat;
                        platName.Text = plat.nom;
                    }
                    else
                    {
                        // Fall back to the previous selected plat and don't update the text
                        box.SelectedItem = _plat;
                        box.SelectedIndex = box.Items.IndexOf(_plat);
                    }
                }
                else
                {
                    _plat = null;
                    platName.Text = string.Empty;
                    // TODO : Remove plat from database
                }
            }
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            comboBox.SelectedIndex = -1;
            comboBox.SelectedItem = null;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            platSelectionPanel.MaxWidth = e.NewSize.Width;
        }
    }
}
