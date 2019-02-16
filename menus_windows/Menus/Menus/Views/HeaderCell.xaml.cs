using System;
using System.Globalization;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                dayText.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(value).UppercaseFirst();
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
