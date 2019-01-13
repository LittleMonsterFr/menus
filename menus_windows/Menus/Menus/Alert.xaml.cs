using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    public sealed partial class Alert : ContentDialog
    {
        public Alert(string title, string message, string text)
        {
            this.InitializeComponent();
            Title = title;
            this.message.Text = message;
            if (text == null)
                scrollView.Visibility = Visibility.Collapsed;
            else
                textBlock.Text = text;
        }
    }
}
