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

        private bool platCanceled = false;

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

                // A meal has been selected in the combobox or the name of a meal is being edited, by the user
                if (box.SelectedItem is Plat || box.SelectedItem is string)
                {
                    // If the name of the meal is being edited
                    if (box.SelectedItem is string newPlatName)
                    {
                        string platNameBackup = _plat.nom;
                        _plat.nom = newPlatName;
                        if (await databaseHandler.EditPlat(_plat) != 1)
                        {
                            _plat.nom = platNameBackup;
                        }
                        else
                        {
                            platName.Text = _plat.nom;
                        }
                    }
                    else
                    {
                        Plat plat = box.SelectedItem as Plat;
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
                        
                }
                else
                {
                    if (platCanceled)
                    {
                        if (await databaseHandler.DeletePlatInSemaine(_plat, Date))
                        {
                            _plat = null;
                            platName.Text = string.Empty;
                        }
                    }

                }
            }
        }

        private void CancelButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            platCanceled = true;
            comboBox.SelectedIndex = -1;
            comboBox.SelectedItem = null;
            platCanceled = false;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            platSelectionPanel.MaxWidth = e.NewSize.Width;
        }
    }
}
