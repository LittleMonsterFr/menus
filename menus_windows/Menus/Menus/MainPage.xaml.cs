using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Menus
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private NavigationTransitionInfo navigationTransitionInfo;
        Dictionary<string, ObservableCollection<Plat>> lists;
        DatabaseHandler databaseHandler;
        int semaine_index = 0;

        public MainPage()
        {
            this.InitializeComponent();
            navigationTransitionInfo = new SlideNavigationTransitionInfo()
            {
                Effect = SlideNavigationTransitionEffect.FromRight
            };
            semaineFrame.Navigate(typeof(Semaine), semaine_index);

            databaseHandler = new DatabaseHandler();
            lists = new Dictionary<string, ObservableCollection<Plat>>();

            List<string> types = databaseHandler.GetTypesPlat();
            if (types != null)
            {
                foreach (string type in types)
                    lists.Add(type, new ObservableCollection<Plat>());

                List<Plat> plats = databaseHandler.GetAllPlat();
                foreach (Plat plat in plats)
                    lists[plat.type].Add(plat);

                entreeList.ItemsSource = lists["Entrée"];
                platResitanceList.ItemsSource = lists["Plat de résistance"];
                soirList.ItemsSource = lists["Soir"];
                dessertList.ItemsSource = lists["Déssert"];
                aperitifList.ItemsSource = lists["Apéritif"];

                entreeTitle.Text = "Entrée";
                platTitle.Text = "Plat de résistance";
                soirtitle.Text = "Soir";
                dessertTitle.Text = "Déssert";
                aperitifTitle.Text = "Apéritif";
            }
        }

        private async void AddPlatButton(object sender, RoutedEventArgs e)
        {
            PlatDialog platDialog = new PlatDialog(databaseHandler)
            {
                Title = "Ajout d'un plat",
                PrimaryButtonText = "Valider",
                CloseButtonText = "Annuler"
            };
            await platDialog.ShowAsync();
        }

        private async void OpenDatabaseFolder(object sender, RoutedEventArgs e)
        {
            StorageFolder databaseFolder = databaseHandler.GetDataBaseFolder();
            await Windows.System.Launcher.LaunchFolderAsync(databaseFolder);
        }

        private async void RemplirDatabase(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".csv");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                List<Tuple<string, string, string>> entries = await Utils.parseCsv(file);
                foreach (Tuple<string, string, string> line in entries)
                {
                    Plat plat = new Plat(0, line.Item2, line.Item1, null, TimeSpan.Zero, 0, null, line.Item3);
                    databaseHandler.InsertPlat(plat);
                }
            }
        }

        public void OnPlatClicked(object sender, ItemClickEventArgs e)
        {
            Plat plat = (Plat) e.ClickedItem;
            Debug.WriteLine(plat.nom);
        }

        private void SemaineNavigation(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name.Equals("back"))
            {
                if (semaineFrame.BackStackDepth > 0)
                    semaineFrame.GoBack();
            }
            else
            {
                semaineFrame.Navigate(typeof(Semaine), ++semaine_index, navigationTransitionInfo);
            }
        }
    }
}
