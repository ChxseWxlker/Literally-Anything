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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class Events : Page
    {
        //create lists of events
        List<Event> currentEvents = new List<Event>();
        List<Event> upcomingEvents = new List<Event>();
        List<Event> pastEvents = new List<Event>();

        //setup current listbox value
        string currentLb = "";

        public Events()
        {
            this.InitializeComponent();
            this.Loaded += Events_Loaded;
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

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }

        private void CreateEvent_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventsCreate));
        }

        private async void Events_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = await Connection.Get("Event");

            //check all events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    //populate upcoming events
                    if (ev.startDate > DateTime.Now)
                    {
                        UpcomingEventsLb.Items.Add(ev.displayName);
                        upcomingEvents.Add(ev);
                    }

                    //populate past events
                    else if (ev.endDate < DateTime.Now)
                    {
                        PastEventsCmb.Items.Add(ev.displayName);
                        pastEvents.Add(ev);
                    }

                    //populate current events
                    else
                    {
                        CurrentEventsLb.Items.Add(ev.displayName);
                        currentEvents.Add(ev);
                    }
                }
        }

        private async void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (currentLb != "")
            {
                //get the event and the current listbox in use
                Event selectedEvent = new Event();

                if (currentLb == "current")
                {
                    foreach (Event ev in currentEvents)
                        if (ev.Id == currentEvents[CurrentEventsLb.SelectedIndex].Id)
                            selectedEvent = ev;
                }

                else if (currentLb == "upcoming")
                {
                    foreach (Event ev in upcomingEvents)
                        if (ev.Id == upcomingEvents[UpcomingEventsLb.SelectedIndex].Id)
                            selectedEvent = ev;
                }

                else if (currentLb == "past")
                {
                    foreach (Event ev in pastEvents)
                        if (ev.Id == pastEvents[PastEventsCmb.SelectedIndex].Id)
                            selectedEvent = ev;
                }

                //navigate to edit page
                Frame.Navigate(typeof(EventsEdit), selectedEvent);
            }
            else
                await new MessageDialog("Please choose an event to edit").ShowAsync();
                
        }

        private async void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (currentLb != "")
            {
                //get selected event from listbox
                Event selectedEvent = new Event();
                if (currentLb == "current")
                    selectedEvent = currentEvents[CurrentEventsLb.SelectedIndex];

                else if (currentLb == "upcoming")
                    selectedEvent = upcomingEvents[UpcomingEventsLb.SelectedIndex];

                else if (currentLb == "past")
                    selectedEvent = pastEvents[PastEventsCmb.SelectedIndex];

                //hide event object
                selectedEvent.hidden = true;

                //edit event object and reload
                Connection.Update(selectedEvent);
                Frame.Navigate(typeof(Events));
            }
            else
                await new MessageDialog("Please choose an event to delete").ShowAsync();
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
