using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CAA_CrossPlatform.UWP.Models;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageGame : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public PageGame()
        {
            this.InitializeComponent();
            this.Loaded += PageGame_Loaded;
        }

        private async void PageGame_Loaded(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = await Connection.Get("Game");

            foreach (Game g in games)
                if (g.hidden == false)
                {
                    lstQuiz.Items.Add(g.name);
                    visibleGames.Add(g);
                }
        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGameCreate));
        }

        private async void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuiz.SelectedIndex == -1)
                await new MessageDialog("Please choose a quiz to edit").ShowAsync();
            else
                Frame.Navigate(typeof(PageGameEdit), visibleGames[lstQuiz.SelectedIndex]);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private async void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuiz.SelectedIndex == -1)
                await new MessageDialog("Please choose a quiz to delete").ShowAsync();
            else
            {
                //hide game object
                visibleGames[lstQuiz.SelectedIndex].hidden = true;

                //edit game object
                Connection.Update(visibleGames[lstQuiz.SelectedIndex]);

                //reload page
                Frame.Navigate(typeof(PageGame));
            }
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //prompt user
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout?",
                Content = "You will be redirected to the home page and locked out until you log back in. Are you sure you want to logout?",
                PrimaryButtonText = "Logout",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult logoutRes = await logoutDialog.ShowAsync();

            //log user out
            if (logoutRes == ContentDialogResult.Primary)
            {
                //reset active username
                Environment.SetEnvironmentVariable("activeUser", "");

                //update menu
                txtAccount.Text = "";

                //logout
                api.Logout();

                //redirect to index
                Frame.Navigate(typeof(PageIndex));
            }
        }

        private void btnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //get menu button
            Button btn = (Button)sender;

            //event
            if (btn.Content.ToString().Contains("Event"))
                Frame.Navigate(typeof(PageEvent));

            //game
            else if (btn.Content.ToString().Contains("Game"))
                Frame.Navigate(typeof(PageGame));

            //question
            else if (btn.Content.ToString().Contains("Question"))
                Frame.Navigate(typeof(PageQuestion));
        }

        private void btnShowPane_Click(object sender, RoutedEventArgs e)
        {
            svMenu.IsPaneOpen = !svMenu.IsPaneOpen;
            if (svMenu.IsPaneOpen)
            {
                btnShowPane.Content = "\uE00E";
                btnEventMenu.Visibility = Visibility.Visible;
                btnGameMenu.Visibility = Visibility.Visible;
                btnQuestionMenu.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPane.Content = "\uE00F";
                btnEventMenu.Visibility = Visibility.Collapsed;
                btnGameMenu.Visibility = Visibility.Collapsed;
                btnQuestionMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void svMenu_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            //hide buttons
            btnShowPane.Content = "\uE00F";
            btnEventMenu.Visibility = Visibility.Collapsed;
            btnGameMenu.Visibility = Visibility.Collapsed;
            btnQuestionMenu.Visibility = Visibility.Collapsed;
        }
    }
}
