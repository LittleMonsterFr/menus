﻿using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        Dictionary<long, ObservableCollection<Plat>> lists;
        DatabaseHandler databaseHandler;
        DateTime date;
        int fadeDuration = 1000;
        ListViewItem selectedItem = null;

        public MainPage()
        {
            this.InitializeComponent();
            forwardTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            backTransition = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
            drillTransition = new DrillInNavigationTransitionInfo();
            date = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            semaineFrame.Navigate(typeof(Semaine), date);

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
        }

        private async void AddPlatButton(object sender, RoutedEventArgs e)
        {
            PlatDialog platDialog = new PlatDialog(databaseHandler, lists)
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

                    Saison saison = new Saison(1, string.Empty);
                    Plat plat = new Plat(0, line.Item2, type, saison, 0, 0, string.Empty, line.Item3);
                    databaseHandler.InsertPlat(plat);
                }
            }
        }

        private void SemaineNavigation(object sender, RoutedEventArgs e)
        {
            if (sender == back)
            {
                semaineFrame.Navigate(typeof(Semaine), date = date.AddDays(-7), backTransition);
            }
            else if (sender == forward)
            {
                semaineFrame.Navigate(typeof(Semaine), date = date.AddDays(7), forwardTransition);
            }
            else if(sender == today)
            {
                semaineFrame.Navigate(typeof(Semaine), date = DateTime.Now.StartOfWeek(DayOfWeek.Monday), drillTransition);
            }
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = (Pivot)sender;
            if (pivot.SelectedItem == platTab)
            {
                today.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                back.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                await forward.Fade(value: 0.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).StartAsync();
                back.Visibility = Visibility.Collapsed;
                forward.Visibility = Visibility.Collapsed;
                today.Visibility = Visibility.Collapsed;

            }
            else if (pivot.SelectedItem == semaineTab)
            {
                today.Visibility = Visibility.Visible;
                back.Visibility = Visibility.Visible;
                forward.Visibility = Visibility.Visible;
                today.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                back.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).Start();
                await forward.Fade(value: 1.0f, duration: fadeDuration, delay: 0, easingType: EasingType.Linear).StartAsync();
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
    }
}
