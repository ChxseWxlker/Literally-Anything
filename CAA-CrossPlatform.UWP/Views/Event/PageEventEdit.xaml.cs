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
    public sealed partial class PageEventEdit : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //create objects
        Event selectedEvent;
        Game selectedGame;

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public PageEventEdit()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get event from previous page
            selectedEvent = (Event)e.Parameter;

            //get all games
            List<Game> games = await Connection.Get("Game");

            //get game
            selectedGame = await Connection.Get("Game", selectedEvent.GameID);

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.name);
                    visibleGames.Add(game);
                }

            //populate info
            EventTxt.Text = selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5);
            StartDateDtp.SelectedDate = selectedEvent.startDate;
            EndDateDtp.SelectedDate = selectedEvent.endDate;
            if (selectedGame != null)
                QuizCmb.SelectedItem = selectedGame.name;
            MemberOnlyChk.IsChecked = selectedEvent.memberOnly;
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            //reset validation template
            EventNameTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            StartDateTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            EndDateTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];

            //validate name
            if (EventTxt.Text == "")
            {
                EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EventTxt.Focus(FocusState.Keyboard);
                return;
            }

            //validate start date
            else if (StartDateDtp.SelectedDate == null)
            {
                StartDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                StartDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //validate end date
            else if (EndDateDtp.SelectedDate == null)
            {
                EndDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //validate date range
            else if (EndDateDtp.SelectedDate <= StartDateDtp.SelectedDate)
            {
                StartDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //fix special characters for sql
            string eventName = EventTxt.Text.Replace("'", "''");

            //setup event record
            selectedEvent.startDate = StartDateDtp.SelectedDate.Value.UtcDateTime;
            selectedEvent.endDate = EndDateDtp.SelectedDate.Value.UtcDateTime;
            selectedEvent.displayName = $"{eventName} {selectedEvent.startDate.Year}";
            selectedEvent.name = selectedEvent.displayName.Replace(" ", "");
            selectedEvent.nameAbbrev = "";
            foreach (string word in eventName.Split(' '))
            {
                char[] letters = word.ToCharArray();
                selectedEvent.nameAbbrev += char.ToUpper(letters[0]);
            }
            selectedEvent.nameAbbrev += $"{selectedEvent.startDate.Month.ToString("00")}{selectedEvent.startDate.Year}";
            selectedEvent.memberOnly = MemberOnlyChk.IsChecked ?? false;
            selectedEvent.GameID = visibleGames[QuizCmb.SelectedIndex].Id;

            //save to database
            Connection.Update(selectedEvent);

            //navigate away
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
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
    }
}
