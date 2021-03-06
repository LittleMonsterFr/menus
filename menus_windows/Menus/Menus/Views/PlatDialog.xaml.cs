﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    public sealed partial class PlatDialog : ContentDialog
    {
        private DatabaseHandler databaseHandler;
        private ObservableCollection<Type> types;
        private ObservableCollection<Saison> saisons;
        private Plat _plat;

        public PlatDialog()
        {
            this.InitializeComponent();
            this.databaseHandler = DatabaseHandler.Instance;
            types = new ObservableCollection<Type>(databaseHandler.Types);
            saisons = new ObservableCollection<Saison>(databaseHandler.Saisons);
        }

        private bool ValidatePlatInput()
        {
            bool res = true;
            SolidColorBrush red = new SolidColorBrush(Windows.UI.Colors.Red);

            if (string.IsNullOrEmpty(nom.Text))
            {
                nom.BorderBrush = red;
                res = false;
            }
            else
                nom.ClearValue(BorderBrushProperty);

            if (type.SelectedValue == null)
            {
                type.BorderBrush = red;
                res = false;
            }
            else
                type.ClearValue(BorderBrushProperty);

            if (saison.SelectedValue == null)
            {
                saison.BorderBrush = red;
                res = false;
            }
            else
                saison.ClearValue(BorderBrushProperty);

            return !res;
        }

        public Plat Plat {
            get { return _plat; }
            set
            {
                _plat = new Plat(value.id, value.Nom, value.type, value.saison, value.Temps, value.note, value.ingredients, value.description);
                nom.Text = value.Nom;
                type.SelectedItem = value.type;
                saison.SelectedItem = value.saison;
                temps.Time = TimeSpan.FromSeconds(value.Temps);
                note.Value = value.note;
                ingredients.Text = value.ingredients;
                description.Text = value.description;
            }

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = ValidatePlatInput();
            if (args.Cancel == false)
            {
                _plat = new Plat(0, nom.Text, (Type)type.SelectedValue, (Saison)saison.SelectedValue, (int)temps.Time.TotalSeconds, (int)note.Value, ingredients.Text, description.Text);
            }
            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = ValidatePlatInput();
            if (args.Cancel == false)
            {
                _plat.Nom = nom.Text;
                _plat.type = (Type)type.SelectedValue;
                _plat.saison = (Saison)saison.SelectedValue;
                _plat.Temps = (int)temps.Time.TotalSeconds;
                _plat.note = (int)note.Value;
                _plat.ingredients = ingredients.Text;
                _plat.description = description.Text;
            }
            deferral.Complete();
        }
    }
}
