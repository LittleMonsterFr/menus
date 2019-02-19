using System;
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

        public Dictionary<long, ObservableCollection<Plat>> Lists { get; set; }
        public DateTime Date { get; set; }
        public ComboBox ComboBox { get; set; }
        public StackPanel MealStackPanel { get; set; }
        public ContentCell SelectedCell { get; set; }

        public Semaine()
        {
            this.InitializeComponent();
        }

        public Semaine(DateTime date, Dictionary<long, ObservableCollection<Plat>> lists)
        {
            this.InitializeComponent();
            Date = date;
            Lists = lists;
            Page_Loaded(null, null);
        }

        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            Date = (DateTime)e.Parameter;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Dictionary<DateTime, Dictionary<long, long>> meals = await DatabaseHandler.Instance.GetPlatIdsForStartOfWeek(Date);
            double minCellWidth = 0;

            UIElementCollection elements = semaineGrid.Children;
            foreach (UIElement element in elements)
            {
                FrameworkElement elt = element as FrameworkElement;
                if (Grid.GetRow(elt) == 0)
                {
                    HeaderCell cell = elt as HeaderCell;
                    cell.Date = Date.AddDays(Grid.GetColumn(cell));
                    cell.Day = (DayOfWeek)((Grid.GetColumn(cell) + 1) % 7);
                    cell.UpdateLayout();
                    if (cell.MinWidth > minCellWidth)
                        minCellWidth = cell.MinWidth;
                }
                else
                {
                    ContentCell contentCell = elt as ContentCell;
                    contentCell.Semaine = this;
                    contentCell.Date = Date.AddDays(Grid.GetColumn(contentCell));
                    if (meals.ContainsKey(contentCell.Date))
                    {
                        Dictionary<long, long> plats = meals[contentCell.Date];
                        if (plats.ContainsKey(Grid.GetRow(elt)))
                        {
                            long plat_id = plats[Grid.GetRow(elt)];
                            foreach (Plat p in Lists[Grid.GetRow(elt)])
                            {
                                if (p.id == plat_id)
                                {
                                    contentCell.Plat = p;
                                    break;
                                }
                            }
                        }
                    }
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

        public void CellSelectedEvent(ContentCell cell)
        {
            // No previous cell selected
            if (SelectedCell == null)
            {
                SelectedCell = cell;
            }
            // The new selected cell is the one already selected
            else if (SelectedCell == cell)
            {
                // Unselect
                cell.OnUnselectedEvent();
                SelectedCell = null;
            }
            // The selected cell is a new one
            else
            {
                // Unselect the previous one
                SelectedCell.OnUnselectedEvent();
                SelectedCell = cell;
            }

            if (SelectedCell != null)
            {
                int line = Grid.GetRow(SelectedCell);
                ComboBox.ItemsSource = Lists[line];
                ComboBox.SelectedItem = SelectedCell.Plat;
                EnableMealStackPanel(true);
            }
            else
            {
                ComboBox.ItemsSource = null;
                EnableMealStackPanel(false);
            }
        }

        public void EnableMealStackPanel(bool enable)
        {
            foreach (Control control in MealStackPanel.Children)
            {
                control.IsEnabled = enable;
            }
        }
    }
}
