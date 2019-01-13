using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        int semaine_index;
        public Semaine()
        {
            this.InitializeComponent();
        }
        override protected void OnNavigatedTo(NavigationEventArgs e)
        {
            semaine_index = (int) e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            semaineIndex.Text = semaine_index.ToString();
        }
    }
}
