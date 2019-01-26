﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        Dictionary<long, ObservableCollection<Plat>> lists;
        DateTime date;

        public Semaine()
        {
            this.InitializeComponent();
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>> pair = (KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>) e.Parameter;
            date = pair.Key;
            lists = pair.Value;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            double minCellWidth = 0;

            UIElementCollection elements = semaineGrid.Children;
            foreach (UIElement element in elements)
            {
                FrameworkElement elt = element as FrameworkElement;
                if (Grid.GetRow(elt) == 0)
                {
                    HeaderCell cell = elt as HeaderCell;
                    cell.Date = date.AddDays(Grid.GetColumn(cell));
                    cell.Day = (DayOfWeek)((Grid.GetColumn(cell) + 1) % 7);
                    cell.UpdateLayout();
                    if (cell.MinWidth > minCellWidth)
                        minCellWidth = cell.MinWidth;
                }
                else
                {
                    ContentCell contentCell = elt as ContentCell;
                    contentCell.ListPlats = lists[Grid.GetRow(elt)];
                }
            }

            semaineGrid.RowDefinitions[0].MinHeight = topLeftCell.MinHeight;
            for (int index = 0; index < semaineGrid.ColumnDefinitions.Count; index++)
            {
                semaineGrid.ColumnDefinitions[index].MinWidth = minCellWidth;
            }
        }

        public void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double cellContentHeight = semaineGrid.ColumnDefinitions[0].ActualWidth;

            UIElementCollection elements = semaineGrid.Children;
            foreach (UIElement element in elements)
            {
                FrameworkElement elt = (FrameworkElement)element;
                if (Grid.GetRow(elt) != 0)
                {
                    ContentCell cell = (ContentCell)elt;
                    cell.Height = cellContentHeight;
                }
            }
        }
    }
}