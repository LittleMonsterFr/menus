using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Semaine : Page
    {
        DateTimeFormatter dateFormatter;
        DateTime date;

        public Semaine()
        {
            this.InitializeComponent();
            dateFormatter = new DateTimeFormatter("day month.full year.full");
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
                    cell.date.Text = dateFormatter.Format(date.AddDays(Grid.GetColumn(cell)));
                }
                else
                {

                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
