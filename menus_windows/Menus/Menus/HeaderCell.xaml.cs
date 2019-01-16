using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Menus
{
    public sealed partial class HeaderCell : UserControl
    {
        int column;

        public HeaderCell()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            column = Grid.GetColumn(this);
            Grid parent = (Grid) this.Parent;

            if (column == 0)
                border.BorderThickness = new Thickness(10, 10, 5, 5);
            else if (column == parent.ColumnDefinitions.Count)
                border.BorderThickness = new Thickness(5, 10, 10, 5);
            else
                border.BorderThickness = new Thickness(5, 10, 10, 5);

            day.Text = ((DayOfWeek) ((column + 1) % 7)).ToString();
        }
    }
}
