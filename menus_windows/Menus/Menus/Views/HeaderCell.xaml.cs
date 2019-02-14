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
        private int column;
        private DateTime _date;
        private DayOfWeek _day;
        private DateTimeFormatter dateFormatter;

        public HeaderCell()
        {
            this.InitializeComponent();
            dateFormatter = new DateTimeFormatter("shortdate");
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
        }

        public DateTime Date {
            get { return _date; }
            set {
                _date = value;
                dateText.Text = dateFormatter.Format(_date);
            }
        }

        public DayOfWeek Day
        {
            get { return _day; }
            set
            {
                _day = value;
                dayText.Text = value.ToString();
            }
        }

        // Look at the stack panel size because it is this size witch is updated once the day and the date have been set
        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MinHeight = e.NewSize.Height + Semaine.doubleBorder * 4;
            MinWidth = e.NewSize.Width + Semaine.doubleBorder * 4;
        }
    }
}
