﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Menus
{
    public sealed partial class ContentCell : UserControl
    {
        Plat plat;
        ObservableCollection<Plat> listPlats;

        public ObservableCollection<Plat> ListPlats
        {
            get { return listPlats; }
            set
            {
                listPlats = value;
                comboBox.ItemsSource = listPlats;
            }
        }

        public ContentCell()
        {
            this.InitializeComponent();
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
            stackPanel.Opacity = 1;
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!comboBox.IsDropDownOpen)
                stackPanel.Opacity = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem is Plat plat)
                platName.Text = plat.nom;
            else
                platName.Text = string.Empty;
        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            comboBox.SelectedIndex = -1;
            comboBox.SelectedItem = null;
        }
    }
}