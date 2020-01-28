using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class EventsCreate : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public EventsCreate()
        {
            this.InitializeComponent();
            this.Loaded += EventsCreate_Loaded;
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private void EventsCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get all games
            List<Game> games = Json.Read("game.json");

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.title);
                    visibleGames.Add(game);
                }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Event gEvent = new Event();
            gEvent.name = EventTxt.Text;
            gEvent.location = LocationTxt.Text;
            gEvent.startDate = Convert.ToDateTime(StartDateDtp.SelectedDate.ToString());
            gEvent.endDate = Convert.ToDateTime(EndDateDtp.SelectedDate.ToString());
            gEvent.game = visibleGames[QuizCmb.SelectedIndex].id;
            gEvent.memberOnly = MemberOnlyChk.IsChecked ?? false;
            gEvent.trackGuestNum = trackGuestChk.IsChecked ?? false;
            gEvent.trackAdultNum = trackAdultChk.IsChecked ?? false;
            gEvent.trackChildNum = trackChildChk.IsChecked ?? false;
            Json.Write(gEvent, "event.json");

            //navigate back to events
            Frame.Navigate(typeof(Events));
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }       
    }
}
