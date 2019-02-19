﻿using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

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

        // Dictionnary with type ids as key and a list of plat corresponding to that type
        private Dictionary<long, ObservableCollection<Plat>> lists;
        private DatabaseHandler databaseHandler;
        private DateTime date;
        private int fadeDuration = 1000;
        private ListViewItem selectedItem = null;
        private PrintHelper printHelper;

        private int ComparePlat(Plat plat1, Plat plat2)
        {
            return string.Compare(plat1.Nom.ToLower(), plat2.Nom.ToLower());
        }

        public MainPage()
        {
            this.InitializeComponent();
            forwardTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            backTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
            drillTransition = new DrillInNavigationTransitionInfo();
            date = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

            // Dumy item to allow selecting only one plat between all the lists
            selectedItem = new ListViewItem();

            databaseHandler = DatabaseHandler.Instance;
            lists = new Dictionary<long, ObservableCollection<Plat>>();

            List<Type> types = databaseHandler.Types;
            foreach (Type type in types)
                lists.Add(type.Id, new ObservableCollection<Plat>());

            List<Plat> plats = databaseHandler.GetPlats().Result;
            foreach (Plat plat in plats)
                lists[plat.type.Id].Add(plat);

            entreeTitle.Text = types[0].Name;
            platTitle.Text = types[1].Name;
            soirtitle.Text = types[2].Name;
            dessertTitle.Text = types[3].Name;
            aperitifTitle.Text = types[4].Name;

            entreeList.ItemsSource = lists[databaseHandler.GetTypeIdForTypeName(entreeTitle.Text).Result].Sort(ComparePlat);
            platResitanceList.ItemsSource = lists[databaseHandler.GetTypeIdForTypeName(platTitle.Text).Result].Sort(ComparePlat);
            soirList.ItemsSource = lists[databaseHandler.GetTypeIdForTypeName(soirtitle.Text).Result].Sort(ComparePlat);
            dessertList.ItemsSource = lists[databaseHandler.GetTypeIdForTypeName(dessertTitle.Text).Result].Sort(ComparePlat);
            aperitifList.ItemsSource = lists[databaseHandler.GetTypeIdForTypeName(aperitifTitle.Text).Result].Sort(ComparePlat);
            
            semaineFrame.Navigate(typeof(Semaine), new KeyValuePair<DateTime, Dictionary<long, ObservableCollection<Plat>>>(date, lists));

            if (PrintManager.IsSupported())
            {
                printButton.IsEnabled = true;
                printHelper = new PrintHelper(this);
            }

            mealSelectionCombobox.ItemsSource = lists[1];
        }

        private async void AddPlatButton(object sender, RoutedEventArgs e)
        {
            PlatDialog platDialog = new PlatDialog()
            {
                Title = "Ajout d'un plat",
                PrimaryButtonText = "Valider",
                CloseButtonText = "Annuler"
            };
            ContentDialogResult result = await platDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Plat plat = platDialog.Plat;
                if (await databaseHandler.InsertPlat(plat))
                {
                    lists[plat.type.Id].Add(plat);
                    lists[plat.type.Id].Sort(ComparePlat);
                }
            }
        }

        private async void OpenDatabaseFolder(object sender, RoutedEventArgs e)
        {
            StorageFolder databaseFolder = databaseHandler.DataBaseFolder;
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
                    long typeId = await databaseHandler.GetTypeIdForTypeName(line.Item1);
                    Type type = new Type(typeId, line.Item1);

                    long saisonId = 1;
                    string saisonName = await databaseHandler.GetSaisonNameForSaisonId(saisonId);
                    Saison saison = new Saison(saisonId, saisonName);
                    Plat plat = new Plat(0, line.Item2, type, saison, 0, 0, string.Empty, line.Item3);
                    if (await databaseHandler.InsertPlat(plat))
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
            else if (pivot.SelectedItem == semainePivotItem)
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
                PlatDialog platDialog = new PlatDialog()
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
                        int res;
                        if ((res = await databaseHandler.EditPlat(platDialog.Plat)) == 1)
                        {
                            int index = lists[plat.type.Id].IndexOf(plat);
                            lists[plat.type.Id].RemoveAt(index);
                            lists[plat.type.Id].Insert(index, platDialog.Plat);
                            selectedItem.DataContext = platDialog.Plat;
                        }
                        else if (res > 1)
                        {
                            await new Alert("Erreur lors de la modification du plat.", "Plus d'un plat a été modifié.", null).ShowAsync();
                        }
                        else
                        {
                            await new Alert("Erreur lors de la modification du plat.", "Une erreur inconnue est survenue.", null).ShowAsync();
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
                    if (await databaseHandler.DeletePlatById(plat.id) == 1)
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

        private void MealSelectionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CancelMealButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
