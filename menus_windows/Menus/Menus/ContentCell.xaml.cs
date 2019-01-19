using System.Diagnostics;
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
        Plat plat;

        public ContentCell()
        {
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int column = Grid.GetColumn(this);
            int row = Grid.GetRow(this);
            Grid parent = (Grid)this.Parent;
            double simpleBorder = Semaine.simpleBorder;
            double doubleBorder = Semaine.doubleBorder;

            if (row != 0)
            {
                double left = column == 0 ? doubleBorder : simpleBorder;
                double top = simpleBorder;
                double right = column == parent.ColumnDefinitions.Count - 1 ? doubleBorder : simpleBorder;
                double bottom = row == parent.RowDefinitions.Count - 1 ? doubleBorder : simpleBorder;

                this.BorderThickness = new Thickness(left, top, right, bottom);
            }
        }

        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Debug.WriteLine("Column {0}, row {1}", Grid.GetColumn(this), Grid.GetRow(this));
        }
    }
}
