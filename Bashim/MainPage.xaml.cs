using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.System;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Bashim
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Bashim.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;
        }

        private void SettingCharmManager_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", OpenPrivacyPolicy));
        }

        private async void OpenPrivacyPolicy(IUICommand command)
        {
            Uri uri = new Uri("http://wintheweb.ru/openprivacy.html");
            await Launcher.LaunchUriAsync(uri);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {

            QuotesSelector.SelectedIndex = 0;
        }

        bool SelectionMode = false;

        private void QS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionMode == true) return;
            SelectionMode = true;

            AbyssSelector.SelectedIndex = -1;
            MoarLove.Visibility = Visibility.Visible;
            switch (QuotesSelector.SelectedIndex)
            {
                case 0:
                    RemoveQuotes();
                    LoadNewQuotes();
                    break;
                case 1:
                    RemoveQuotes();
                    LoadRandomQuotes();
                    break;
            }


            SelectionMode = false;
        }

        private void AS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionMode == true) return;
            SelectionMode = true;

            QuotesSelector.SelectedIndex = -1;

            switch (AbyssSelector.SelectedIndex)
            {
                case 0:
                    MoarLove.Visibility = Visibility.Visible;
                    RemoveQuotes();
                    LoadAbyss();
                    break;
                case 1:
                    MoarLove.Visibility = Visibility.Collapsed;
                    RemoveQuotes();
                    LoadTopAbyss();                    
                    break;
            }

            SelectionMode = false;
        }

        BashOrgRu bashQuotes = new BashOrgRu();

        FontFamily Consolas = new FontFamily("Consolas");

        int lastPageLoaded;

        SolidColorBrush lgb = new SolidColorBrush(Windows.UI.ColorHelper.FromArgb(255, 241, 241, 241));

        Thickness Twelve = new Thickness(12);

        void AddQuote(string quote)
        {
            TextBlock t = new TextBlock { Width = 600, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 14, Text = quote, Margin = Twelve, FontFamily = Consolas };
            TextBlock t2 = new TextBlock { Width = 270, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Left, FontSize = 12, Text = quote, Margin = Twelve, FontFamily = Consolas };
            QuotesViewer.Children.Insert(QuotesViewer.Children.Count - 1, new Border { Background = lgb, Margin = Twelve, Child = t, Width = 800 });
            QuotesViewerSnapped.Children.Insert(QuotesViewerSnapped.Children.Count - 1, new Border { Background = lgb, Margin = Twelve, Child = t2, Width = 350 });
        }

        private async void LoadNewQuotes(bool More = false)
        {
            try
            {
                if (More == true)
                {
                    var q = await bashQuotes.GetNewQuotes(--lastPageLoaded);

                    foreach (var quote in q)
                        AddQuote(quote.quote);

                    return;
                }

                lastPageLoaded = bashQuotes.PagesCount;
                foreach (var quote in await bashQuotes.GetNewQuotes()) AddQuote(quote.quote);
            }
            catch
            {
                new MessageDialog("Во время получения новых цитат произошла какая-то ошибка. Возможно, сломались интернеты. Если данная ошибка повторится, обратитесь к разработчику.", "Bash.im").ShowAsync();
            }
        }

        private async void LoadRandomQuotes()
        {
            try
            {
                var RandomQuotes = await bashQuotes.GetRandomQuotes();

                foreach (var quote in RandomQuotes)
                    AddQuote(quote.quote);
            }
            catch
            {
                new MessageDialog("Во время получения новых цитат произошла какая-то ошибка. Возможно, сломались интернеты. Если данная ошибка повторится, обратитесь к разработчику.", "Bash.im").ShowAsync();
            }
        }

        private async void LoadAbyss()
        {
            try
            {
                var AbyssQuotes = await bashQuotes.GetAbyss();

                foreach (var quote in AbyssQuotes)
                    AddQuote(quote.quote);
            }
            catch
            {
                new MessageDialog("Во время получения новых цитат произошла какая-то ошибка. Возможно, сломались интернеты. Если данная ошибка повторится, обратитесь к разработчику.", "Bash.im").ShowAsync();
            }
        }

        private async void LoadTopAbyss()
        {
            try
            {
                var AbyssQuotes = await bashQuotes.GetAbyssTop();

                foreach (var quote in AbyssQuotes)
                    AddQuote(quote.quote);
            }
            catch
            {
                new MessageDialog("Во время получения новых цитат произошла какая-то ошибка. Возможно, сломались интернеты. Если данная ошибка повторится, обратитесь к разработчику.", "Bash.im").ShowAsync();
            }
        }

        private void RemoveQuotes()
        {
            for (int i = QuotesViewer.Children.Count - 2; i >= 0; i--)
            {
                QuotesViewer.Children.RemoveAt(i);
                QuotesViewerSnapped.Children.RemoveAt(i);
            }
        }


        private void MoarLove_Click(object sender, RoutedEventArgs e)
        {
            if (QuotesSelector.SelectedIndex == 0)
                LoadNewQuotes(true);
            else if (QuotesSelector.SelectedIndex == 1)
            { 
                RemoveQuotes(); 
                LoadRandomQuotes();
            }
            else if (AbyssSelector.SelectedIndex == 0)
            {
                RemoveQuotes();
                LoadAbyss();
            }
        }


    }
}
