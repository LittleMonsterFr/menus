using System;
using System.Diagnostics;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Semaine : Page
    {
        public static double simpleBorder = 2.5;
        public static double doubleBorder = simpleBorder * 2;
        
        DateTime date;

        public Semaine()
        {
            this.InitializeComponent();
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            date = (DateTime) e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double height = semaineGrid.ColumnDefinitions[0].ActualWidth;
            double minHeaderRowHeight = 0;
            double minColumnWidth = 0;

            UIElementCollection elements = semaineGrid.Children;
            foreach (UIElement element in elements)
            {
                FrameworkElement elt = (FrameworkElement)element;
                if (Grid.GetRow(elt) == 0)
                {
                    HeaderCell cell = (HeaderCell)elt;
                    cell.Date = date.AddDays(Grid.GetColumn(cell));
                    minHeaderRowHeight = cell.MinHeight;
                    if (cell.MinWidth > minColumnWidth)
                        minColumnWidth = cell.MinWidth;
                }
                else
                {
                    ContentCell cell = (ContentCell)elt;
                    cell.Height = height;
                }
            }

            semaineGrid.RowDefinitions[0].MinHeight = minHeaderRowHeight;
            for (int index = 0; index < semaineGrid.ColumnDefinitions.Count; index++)
            {
                semaineGrid.ColumnDefinitions[index].MinWidth = minColumnWidth;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = semaineGrid.ColumnDefinitions[0].ActualWidth;

            UIElementCollection elements = semaineGrid.Children;
            foreach (UIElement element in elements)
            {
                FrameworkElement elt = (FrameworkElement)element;
                if (Grid.GetRow(elt) != 0)
                {
                    ContentCell cell = (ContentCell)elt;
                    cell.Height = height;
                }
            }
        }
    }
}
