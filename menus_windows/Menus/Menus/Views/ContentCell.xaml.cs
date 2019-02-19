using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Menus
{
    public sealed partial class ContentCell : UserControl
    {
        private Brush defaultBrush;
        private Plat _plat;

        public Semaine Semaine { get; set; }

        // Public reference coresponding to the colum this cell is in
        public DateTime Date { get; set; }

        public Plat Plat
        {
            get { return _plat; }
            set
            {
                _plat = value;
                platName.Text = _plat != null ? value.Nom : string.Empty;
            }
        }

        // Default constructor
        public ContentCell()
        {
            this.InitializeComponent();
        }

        // Defines the border at loading time
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int column = Grid.GetColumn(this);
            int row = Grid.GetRow(this);
            Grid parent = (Grid)this.Parent;
            double simpleBorder = Semaine.simpleBorder;
            double doubleBorder = Semaine.doubleBorder;

            double left = column == 0 ? doubleBorder : simpleBorder;
            double top = simpleBorder;
            double right = column == parent.ColumnDefinitions.Count - 1 ? doubleBorder : simpleBorder;
            double bottom = row == parent.RowDefinitions.Count - 1 ? doubleBorder : simpleBorder;

            border.BorderThickness = new Thickness(left, top, right, bottom);

            defaultBrush = border.BorderBrush;
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private void ContentCell_Tapped(object sender, TappedRoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            Semaine.CellSelectedEvent(this);
        }

        public void OnUnselectedEvent()
        {
            border.BorderBrush = defaultBrush;
        }
    }
}
