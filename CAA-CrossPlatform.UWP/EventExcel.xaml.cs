using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class EventExcel
    {
        List<Event> visibleEvents = new List<Event>();

        public EventExcel()
        {
            this.InitializeComponent();
            this.Loaded += EventExcel_Loaded;
        }

        private void EventExcel_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = Json.Read("event.json");

            //create list of visible events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    lstEvents.Items.Add(ev.name);
                    visibleEvents.Add(ev);
                }
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

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //get list of events
            List<Event> eventsJSON = Json.Read("event.json");
            List<Event> events = await Excel.Load();
            bool eventExist = false;

            foreach (Event ev in events)
            {                
                foreach (Event evJ in eventsJSON) {
                    if (ev.id == evJ.id)
                        eventExist = true;
                }
                if ((ev.hidden == false) && (eventExist == false))
                {
                    lstEvents.Items.Add(ev.name);
                    visibleEvents.Add(ev);

                    //create event object
                    Event gEvent = new Event();

                    //set object properties
                    gEvent.name = ev.name;
                    gEvent.location = ev.location;
                    gEvent.startDate = ev.startDate;
                    gEvent.endDate = ev.endDate;
                    gEvent.game = ev.game;
                    gEvent.memberOnly = ev.memberOnly;
                    gEvent.trackGuestNum = ev.trackGuestNum;
                    gEvent.trackAdultNum = ev.trackAdultNum;
                    gEvent.trackChildNum = ev.trackChildNum;

                    //save json to file
                    Json.Write(gEvent, "event.json");
                }
                eventExist = false;
            }

            /*if (events.Count != 0)
                await new MessageDialog(eventsStr).ShowAsync();*/
            
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //validation
            if (lstEvents.SelectedItems.Count == 0)
            {
                await new MessageDialog("No events selected, please choose one").ShowAsync();
                return;
            }

            //create list of selected events
            List<Event> selectedEvents = new List<Event>();

            //add to events
            foreach (string evStr in lstEvents.SelectedItems)
                foreach (Event ev in visibleEvents)
                    if (evStr == ev.name)
                        selectedEvents.Add(ev);

            //save to excel spreadsheet
            Excel.Save(selectedEvents);

            //show message output
            await new MessageDialog("Events imported").ShowAsync();
        }

        private void chkAllEvents_Checked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectAll();
        }

        private void chkAllEvents_Unchecked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectedIndex = -1;
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
    }
}
