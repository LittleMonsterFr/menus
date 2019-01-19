using System;
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
            UIElementCollection elements = semaineGrid.Children;
            foreach(UIElement element in elements)
            {
                FrameworkElement elt = (FrameworkElement)element;
                if (Grid.GetRow(elt) == 0)
                {
                    HeaderCell cell = (HeaderCell)elt;
                    cell.Date = date.AddDays(Grid.GetColumn(cell));
                }
                else
                {

                }
            }
        }
    }
}
