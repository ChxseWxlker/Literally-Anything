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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class Events : Page
    {
        public Events()
        {
            this.InitializeComponent();
            this.Loaded += Events_Loaded;
        }

        List<Event> currentEvents = new List<Event>();
        List<Event> upcomingEvents = new List<Event>();
        List<Event> pastEvents = new List<Event>();
        string currentLb = "";

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

        private void Events_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = Json.Read("event.json");

            //populate current events
            foreach (Event gEvent in events)
                if (gEvent.startDate <= DateTime.Now && gEvent.endDate >= DateTime.Now && gEvent.hidden == false)
                {
                    CurrentEventsLb.Items.Add(gEvent.name);
                    currentEvents.Add(gEvent);
                }

            //populate upcoming events
            foreach (Event gEvent in events)
                if (gEvent.startDate > DateTime.Now && gEvent.hidden == false)
                {
                    UpcomingEventsLb.Items.Add(gEvent.name);
                    upcomingEvents.Add(gEvent);
                }

            //populate past events
            foreach (Event gEvent in events)
                if (gEvent.endDate < DateTime.Now && gEvent.hidden == false)
                {
                    PastEventsCmb.Items.Add(gEvent.name);
                    pastEvents.Add(gEvent);
                }
        }

        private void CreateEvent_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventsCreate));
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            int eventID = -1;
            if (currentLb == "current")
            {
                foreach (Event ev in currentEvents)
                    if (ev.id == currentEvents[CurrentEventsLb.SelectedIndex].id)
                        eventID = ev.id;
            }
            else if (currentLb == "upcoming")
            {
                foreach (Event ev in upcomingEvents)
                    if (ev.id == upcomingEvents[UpcomingEventsLb.SelectedIndex].id)
                        eventID = ev.id;
            }
            else if (currentLb == "past")
            {
                foreach (Event ev in pastEvents)
                    if (ev.id == pastEvents[PastEventsCmb.SelectedIndex].id)
                        eventID = ev.id;
            }

            Frame.Navigate(typeof(EventsEdit), eventID);
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CurrentEventsLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLb = "current";
            UpcomingEventsLb.SelectedIndex = -1;
            PastEventsCmb.SelectedIndex = -1;
        }

        private void UpcomingEventsLb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLb = "upcoming";
            CurrentEventsLb.SelectedIndex = -1;
            PastEventsCmb.SelectedIndex = -1;
        }

        private void PastEventsCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentLb = "past";
            CurrentEventsLb.SelectedIndex = -1;
            UpcomingEventsLb.SelectedIndex = -1;
        }
    }
}
