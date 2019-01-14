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
            dateFormatter = new DateTimeFormatter("dayofweek.full day month.full year.full");
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            date = (DateTime) e.Parameter;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            for (int c = 0; c < 7; c++)
            {
                for (int l = 0; l < 3; l++)
                {
                    Border border = new Border();
                    if (l == 0)
                    {
                        border.VerticalAlignment = VerticalAlignment.Center;
                        border.HorizontalAlignment = HorizontalAlignment.Center;
                        TextBlock textBlock = new TextBlock
                        {
                            Text = dateFormatter.Format(date)
                        };
                        border.Child = textBlock;
                        Grid.SetColumn(border, c);
                        Grid.SetRow(border, l);
                        semaineGrid.
                    }
                }
            }
        }
    }
}
