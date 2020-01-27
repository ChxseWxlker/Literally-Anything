﻿using System;
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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class EventsEdit : Page
    {
        //create event object
        Event gEvent;

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public EventsEdit()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
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

            //get event from previous page
            List<Event> events = Json.Read("event.json");
            foreach (Event ev in events)
                if (ev.id == Convert.ToInt32(e.Parameter))
                    gEvent = ev;

            //populate info
            EventTxt.Text = gEvent.name;
            LocationTxt.Text = gEvent.location;
            StartDateDtp.SelectedDate = gEvent.startDate;
            EndDateDtp.SelectedDate = gEvent.endDate;
            foreach (Game game in games)
                if (game.id == gEvent.id)
                    QuizCmb.SelectedItem = game.title;
            MemberOnlyChk.IsChecked = gEvent.memberOnly;
            trackGuestChk.IsChecked = gEvent.trackGuestNum;
            trackAdultChk.IsChecked = gEvent.trackAdultNum;
            trackChildChk.IsChecked = gEvent.trackChildNum;
            
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

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }
    }
}
