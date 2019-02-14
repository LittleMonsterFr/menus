using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
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
        private NavigationTransitionInfo backTransition;
        private NavigationTransitionInfo forwardTransition;
        private NavigationTransitionInfo drillTransition;
        private Dictionary<long, ObservableCollection<Plat>> lists;
        private DatabaseHandler databaseHandler;
        private DateTime date;
        private int fadeDuration = 1000;
        private ListViewItem selectedItem = null;
        private PrintHelper printHelper;

        public MainPage()
        {
            this.InitializeComponent();
            forwardTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            backTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
            drillTransition = new DrillInNavigationTransitionInfo();
            date = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            //Dumy item
            selectedItem = new ListViewItem();

            databaseHandler = new DatabaseHandler();
            lists = new Dictionary<long, ObservableCollection<Plat>>();

            List<Type> types = databaseHandler.GetTypes();

            foreach (Type type in types)
                lists.Add(type.Id, new ObservableCollection<Plat>());

            List<Plat> plats = databaseHandler.GetPlats();
            foreach (Plat plat in plats)
                lists[plat.type.Id].Add(plat);

            entreeList.ItemsSource = lists[databaseHandler.GetIdForTypeName("Entrée")];
            platResitanceList.ItemsSource = lists[databaseHandler.GetIdForTypeName("Plat de résistance")];
            soirList.ItemsSource = lists[databaseHandler.GetIdForTypeName("Soir")];
            dessertList.ItemsSource = lists[databaseHandler.GetIdForTypeName("Déssert")];
            aperitifList.ItemsSource = lists[databaseHandler.GetIdForTypeName("Apéritif")];

            entreeTitle.Text = "Entrée";
            platTitle.Text = "Plat de résistance";
            soirtitle.Text = "Soir";
            dessertTitle.Text = "Déssert";
            aperitifTitle.Text = "Apéritif";

            semaineFrame.Navigate(typeof(Semaine), new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date, lists));

            if (PrintManager.IsSupported())
            {
                printButton.IsEnabled = true;
                printHelper = new PrintHelper(this);
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
            ContentDialogResult result = await platDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Plat plat = platDialog.Plat;
                if (databaseHandler.InsertPlat(plat) == true)
                {
                    lists[plat.type.Id].Add(plat);
                }
            }
        }

        private async void OpenDatabaseFolder(object sender, RoutedEventArgs e)
        {
            StorageFolder databaseFolder = databaseHandler.GetDataBaseFolder();
            await Windows.System.Launcher.LaunchFolderAsync(databaseFolder);
        }

        private async void RemplirDatabase(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
            };
            picker.FileTypeFilter.Add(".csv");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                List<Tuple<string, string, string>> entries = await Utils.parseCsv(file);
                foreach (Tuple<string, string, string> line in entries)
                {
                    long typeId = databaseHandler.GetIdForTypeName(line.Item1);
                    Type type = new Type(typeId, line.Item1);

                    long saisonId = 1;
                    string saisonName = databaseHandler.GetSaisonNameForId(saisonId);
                    Saison saison = new Saison(saisonId, saisonName);
                    Plat plat = new Plat(0, line.Item2, type, saison, 0, 0, string.Empty, line.Item3);
                    if (databaseHandler.InsertPlat(plat))
                    {
                        lists[plat.type.Id].Add(plat);
                    }
                }
            }
        }

        private void SemaineNavigation(object sender, RoutedEventArgs e)
        {
            printHelper.UnregisterForPrinting();
            if (sender == back)
            {
                semaineFrame.Navigate(typeof(Semaine), new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date = date.AddDays(-7), lists), backTransition);
            }
            else if (sender == forward)
            {
                semaineFrame.Navigate(typeof(Semaine), new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date = date.AddDays(7), lists), forwardTransition);
            }
            else if(sender == today)
            {
                semaineFrame.Navigate(typeof(Semaine), new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date = DateTime.Now.StartOfWeek(DayOfWeek.Monday), lists), drillTransition);
            }
            printHelper.RegisterForPrinting();
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = (Pivot)sender;
            if (pivot.SelectedItem == platTab)
            {
                printButton.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                today.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                back.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                await forward.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).StartAsync();
                printButton.Visibility = Visibility.Collapsed;
                back.Visibility = Visibility.Collapsed;
                forward.Visibility = Visibility.Collapsed;
                today.Visibility = Visibility.Collapsed;
                printHelper.UnregisterForPrinting();
            }
            else if (pivot.SelectedItem == semaineTab)
            {
                printButton.Visibility = Visibility.Visible;
                today.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Visible;
                forward.Visibility = Visibility.Visible;
                printButton.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                today.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                back.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                await forward.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).StartAsync();
                printHelper.RegisterForPrinting();
            }
        }

        private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            selectedItem.IsSelected = false;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            selectedItem = sender as ListViewItem;
        }

        private void ListViewItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            selectedItem.IsSelected = false;
            selectedItem = (sender as ListViewItem);
            selectedItem.IsSelected = true;
        }

        private async void MenuFlyoutItem_Tapped(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            string text = item.Text;
            if (text.Equals("Modifier"))
            {
                PlatDialog platDialog = new PlatDialog(databaseHandler)
                {
                    Title = "Edition d'un plat",
                    SecondaryButtonText = "Modifier",
                    CloseButtonText = "Annuler"
                };
                Plat plat = selectedItem.DataContext as Plat;
                platDialog.Plat = plat;
                ContentDialogResult result = await platDialog.ShowAsync();
                if (result == ContentDialogResult.Secondary)
                {
                    if (platDialog.Plat.Equals(plat) == false)
                    {
                        if (databaseHandler.EditPlat(platDialog.Plat) == 1)
                        {
                            int index = lists[plat.type.Id].IndexOf(plat);
                            lists[plat.type.Id].RemoveAt(index);
                            lists[plat.type.Id].Insert(index, platDialog.Plat);
                            selectedItem.DataContext = platDialog.Plat;
                        }
                        else
                        {
                            await new Alert("Erreur lors de la modification du plat.", "Plus d'un plat a été modifié.", null).ShowAsync();
                        }
                    }
                }
            }
            else if (text.Equals("Supprimer"))
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Suppression d'un plat",
                    PrimaryButtonText = "Oui ! 😃",
                    SecondaryButtonText = "Non ! 😱",
                };

                dialog.Content = new TextBlock()
                {
                    Text = "Confirmez-vous la suppression du plat ?",
                };

                ContentDialogResult result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Plat plat = selectedItem.DataContext as Plat;
                    if (databaseHandler.DeletePlatById(plat.id) == 1)
                    {
                        foreach (ObservableCollection<Plat> ListPlat in lists.Values)
                        {
                            if (ListPlat.Contains(plat))
                            {
                                ListPlat.Remove(plat);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private async void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            Page semainePage = new Semaine(new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date, lists));
            printHelper.PreparePrintContent(semainePage);
            await printHelper.ShowPrintUIAsync();
        }
    }
}
