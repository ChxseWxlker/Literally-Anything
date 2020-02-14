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
    public sealed partial class EventsEdit : Page
    {
        //create objects
        Event selectedEvent;
        Game selectedGame;
        EventGame selectedEventGame;

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public EventsEdit()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get event from previous page
            selectedEvent = (Event)e.Parameter;

            //get all games
            List<Game> games = await Connection.Get("Game");

            //get event game
            List<EventGame> eventGames = await Connection.Get("EventGame");
            foreach (EventGame eg in eventGames)
                if (eg.EventID == selectedEvent.Id)
                {
                    selectedGame = await Connection.Get("Game", eg.GameID);
                    selectedEventGame = eg;
                }

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.title);
                    visibleGames.Add(game);
                }

            //populate info
            EventTxt.Text = selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5);
            StartDateDtp.SelectedDate = selectedEvent.startDate;
            EndDateDtp.SelectedDate = selectedEvent.endDate;
            if (selectedGame != null)
                QuizCmb.SelectedItem = selectedGame.title;
            MemberOnlyChk.IsChecked = selectedEvent.memberOnly;
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
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

            //save to database
            Connection.Update(selectedEvent);

            //setup event game record
            if (selectedEventGame != null)
                selectedEventGame.GameID = visibleGames[QuizCmb.SelectedIndex].Id;

            //save to database
            if (selectedEventGame != null)
                Connection.Update(selectedEventGame);

            //navigate away
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }
    }
}
