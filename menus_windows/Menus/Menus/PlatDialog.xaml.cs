﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menus
{
    public sealed partial class PlatDialog : ContentDialog
    {
        private DatabaseHandler databaseHandler;

        public PlatDialog(DatabaseHandler databaseHandler)
        {
            this.InitializeComponent();
            this.databaseHandler = databaseHandler;
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

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = ValidatePlatInput();
            if (args.Cancel == false)
            {
                Plat plat = new Plat(0, nom.Text, (string) type.SelectedValue, (string)saison.SelectedValue, temps.Time, (int) note.Value, ingredients.Text, description.Text);
                Exception e = databaseHandler.InsertPlat(plat);
                if (e != null)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            deferral.Complete();
        }
    }
}
