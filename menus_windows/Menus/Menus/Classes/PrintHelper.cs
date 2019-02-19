using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Printing;

namespace Menus
{
    public class PrintHelper
    {
        /// <summary>
        /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
        /// </summary>
        protected double ApplicationContentMarginLeft = 0.05;

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        protected double ApplicationContentMarginTop = 0.05;

        /// <summary>
        /// PrintDocument is used to prepare the pages for printing.
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        protected PrintDocument printDocument;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        protected IPrintDocumentSource printDocumentSource;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>
        internal List<UIElement> printPreviewPages;

        // Event callback which is called after print preview pages are generated.  Photos scenario uses this to do filtering of preview pages
        protected event EventHandler PreviewPagesCreated;

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        protected FrameworkElement firstPage;

        /// <summary>
        ///  A reference back to the scenario page used to access XAML elements on the scenario page
        /// </summary>
        protected Page scenarioPage;

        /// <summary>
        ///  A hidden canvas used to hold pages we wish to print
        /// </summary>
        protected Canvas PrintCanvas
        {
            get
            {
                return scenarioPage.FindName("PrintCanvas") as Canvas;
            }
        }

        protected int FontSize = 14;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scenarioPage">The scenario page constructing us</param>
        public PrintHelper(Page scenarioPage)
        {
            this.scenarioPage = scenarioPage;
            printPreviewPages = new List<UIElement>();
        }

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        public virtual void RegisterForPrinting()
        {
            if (printDocument == null)
            {
                printDocument = new PrintDocument();
                printDocumentSource = printDocument.DocumentSource;
            }
            
            printDocument.Paginate += CreatePrintPreviewPages;
            printDocument.GetPreviewPage += GetPrintPreviewPage;
            printDocument.AddPages += AddPrintPages;

            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested += PrintTaskRequested;
        }

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        public virtual void UnregisterForPrinting()
        {
            if (printDocument == null)
            {
                return;
            }

            printDocument.Paginate -= CreatePrintPreviewPages;
            printDocument.GetPreviewPage -= GetPrintPreviewPage;
            printDocument.AddPages -= AddPrintPages;

            // Remove the handler for printing initialization.
            PrintManager printManager = PrintManager.GetForCurrentView();
            printManager.PrintTaskRequested -= PrintTaskRequested;

            PrintCanvas.Children.Clear();
        }

        public async Task ShowPrintUIAsync()
        {
            // Catch and print out any errors reported
            try
            {
                await PrintManager.ShowPrintUIAsync();
            }
            catch (Exception e)
            {
                await new Alert("Erreur d'impression", "Une erreur lors de la preparation de l'impression est survenue.", e.StackTrace).ShowAsync();
            }
        }

        /// <summary>
        /// Method that will generate print content for the scenario
        /// For scenarios 1-4: it will create the first page from which content will flow
        /// Scenario 5 uses a different approach
        /// </summary>
        /// <param name="page">The page to print</param>
        public virtual void PreparePrintContent(Page page)
        {

            firstPage = page;

            // Add the (newly created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(firstPage);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();
        }

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs </param>
        protected virtual void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;
            Semaine semainePage = firstPage as Semaine;
            printTask = e.Request.CreatePrintTask("Menus - Semaine du " + semainePage.Date.ToShortDateString(), sourceRequestedArgs =>
            {
                var deferral = sourceRequestedArgs.GetDeferral();

                PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);
                IList<string> displayedOptions = printDetailedOptions.DisplayedOptions;

                // Choose the printer options to be shown, if the printer support them
                // The order in which the options are appended determines the order in which they appear in the UI
                displayedOptions.Clear();

                displayedOptions.Add(StandardPrintTaskOptions.Copies);
                displayedOptions.Add(StandardPrintTaskOptions.Orientation);
                displayedOptions.Add(StandardPrintTaskOptions.ColorMode);

                PrintCustomItemListOptionDetails fontSizes = printDetailedOptions.CreateItemListOption("fontSizes", "Taille de police");
                for (int i = 1; i <= 25; i++)
                {
                    fontSizes.AddItem("font_" + i.ToString(), i.ToString());
                }

                // App tells the user some more information about what the feature means.
                fontSizes.Description = "Détermine la taille de police lors de l'impression.";

                // Set the default value
                fontSizes.TrySetValue("font_" + FontSize.ToString());

                // Add the custom option to the option list
                displayedOptions.Add("fontSizes");

                printDetailedOptions.OptionChanged += printDetailedOptions_OptionChanged;

                // Print Task event handler is invoked when the print job is completed.
                printTask.Completed += async (s, args) =>
                {
                    // Notify the user when the print operation fails.
                    if (args.Completion == PrintTaskCompletion.Failed)
                    {
                        await scenarioPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                        {
                            await new Alert("Erreur d'impression", "Une erreur est survenue lors de l'impression.", null).ShowAsync();
                        });
                    }
                };

                sourceRequestedArgs.SetSource(printDocumentSource);

                deferral.Complete();
            });
        }

        async void printDetailedOptions_OptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            bool invalidatePreview = false;

            string optionId = args.OptionId as string;
            if (string.IsNullOrEmpty(optionId))
            {
                return;
            }

            if (optionId == "fontSizes")
            {
                PrintCustomItemListOptionDetails fontSizes = (PrintCustomItemListOptionDetails)sender.Options["fontSizes"];
                FontSize = int.Parse(fontSizes.Value.ToString().Split('_')[1]);
                
                invalidatePreview = true;
            }

            if (invalidatePreview)
            {
                await scenarioPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    printDocument.InvalidatePreview();
                });
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        protected virtual void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            lock (printPreviewPages)
            {
                // Clear the cache of preview pages
                printPreviewPages.Clear();

                // Clear the print canvas of preview pages
                PrintCanvas.Children.Clear();

                // Get the PrintTaskOptions
                PrintTaskOptions printingOptions = e.PrintTaskOptions;

                // Get the page description to deterimine how big the page is
                PrintPageDescription pageDescription = printingOptions.GetPageDescription(0);

                AddPrintPreviewPage(pageDescription);

                if (PreviewPagesCreated != null)
                {
                    PreviewPagesCreated.Invoke(printPreviewPages, null);
                }

                PrintDocument printDoc = (PrintDocument)sender;

                // Report the number of preview pages created
                printDoc.SetPreviewPageCount(printPreviewPages.Count, PreviewPageCountType.Intermediate);
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        protected async virtual void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            try
            {
                PrintDocument printDoc = (PrintDocument)sender;
                printDoc.SetPreviewPage(e.PageNumber, printPreviewPages[e.PageNumber - 1]);
            }
            catch (Exception ex)
            {
                await new Alert("Erreur lors de la génération du rendu.", "Une erreur inconnue est survenue.", ex.StackTrace).ShowAsync();
            }
        }

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        protected virtual void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            // Loop over all of the preview pages and add each one to  add each page to be printied
            for (int i = 0; i < printPreviewPages.Count; i++)
            {
                // We should have all pages ready at this point...
                printDocument.AddPage(printPreviewPages[i]);
            }

            PrintDocument printDoc = (PrintDocument)sender;

            // Indicate that all of the print pages have been provided
            printDoc.AddPagesComplete();
        }

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        protected virtual void AddPrintPreviewPage(PrintPageDescription printPageDescription)
        {
            // XAML element that is used to represent to "printing page"
            FrameworkElement page = firstPage;
            (page as Semaine).FontSize = FontSize;
            
            // Set "paper" width
            page.Width = printPageDescription.PageSize.Width;
            page.Height = printPageDescription.PageSize.Height;

            RelativePanel semainePanel = (RelativePanel)page.FindName("semainePanel");
            Grid semaineGrid = (Grid)page.FindName("semaineGrid");
            RelativePanel.SetAlignBottomWithPanel(semaineGrid, false);

            // Get the margins size
            // If the ImageableRect is smaller than the app provided margins use the ImageableRect
            double marginWidth = Math.Max(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
            double marginHeight = Math.Max(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

            // Set-up "printable area" on the "paper"
            semainePanel.Width = page.Width - marginWidth;
            semainePanel.Height = page.Height - marginHeight;

            // Add the (newley created) page to the print canvas which is part of the visual tree and force it to go
            // through layout so that the linked containers correctly distribute the content inside them.
            PrintCanvas.Children.Add(page);
            PrintCanvas.InvalidateMeasure();
            PrintCanvas.UpdateLayout();

            // Add the page to the page preview collection
            printPreviewPages.Add(page);
        }
    }
}
