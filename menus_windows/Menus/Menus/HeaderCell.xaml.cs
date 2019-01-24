using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
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
        DateTime date;
        DateTimeFormatter dateFormatter;
        double minWidth = 0;

        public HeaderCell()
        {
            this.InitializeComponent();
            dateFormatter = new DateTimeFormatter("day month.full year.full");
        }

        private void HeaderCell_Loaded(object sender, RoutedEventArgs e)
        {
            column = Grid.GetColumn(this);
            Grid parent = (Grid)this.Parent;
            double simpleBorder = Semaine.simpleBorder;
            double doubleBorder = Semaine.doubleBorder;

            if (column == 0)
                border.BorderThickness = new Thickness(doubleBorder, doubleBorder, simpleBorder, simpleBorder);
            else if (column == parent.ColumnDefinitions.Count - 1)
                border.BorderThickness = new Thickness(simpleBorder, doubleBorder, doubleBorder, simpleBorder);
            else
                border.BorderThickness = new Thickness(simpleBorder, doubleBorder, simpleBorder, simpleBorder);

            MinHeight = stackPanel.ActualHeight + doubleBorder * 4;
            dayText.Text = ((DayOfWeek)((column + 1) % 7)).ToString();
        }

        public DateTime Date {
            get { return date; }
            set {
                date = value;
                dateText.Text = dateFormatter.Format(date);
            }
        }

        private void Text_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock.ActualWidth > minWidth)
                MinWidth = textBlock.ActualWidth + Semaine.doubleBorder * 4;
        }
    }
}
