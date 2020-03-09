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
    public sealed partial class PageEvent : Page
    {
        //create lists of events
        List<Event> activeEvents = new List<Event>();
        List<Event> upcomingEvents = new List<Event>();
        List<Event> pastEvents = new List<Event>();

        //create selected event
        Event selectedEvent = new Event();

        //setup scroller vertical offset
        double scrollerVerticalOffset = 0;

        public PageEvent()
        {
            this.InitializeComponent();
            this.Loaded += PageEvent_Loaded;
        }

        private async void PageEvent_Loaded(object sender, RoutedEventArgs e)
        {
            //reset environment vars
            EnvironmentModel.Reset();

            //get permissions
            if (Environment.GetEnvironmentVariable("activeUser") == "Guest")
                btnCreateEvent.Visibility = Visibility.Collapsed;

            else
                btnCreateEvent.Visibility = Visibility.Visible;

            //get all events
            List<Event> events = await Connection.Get("Event");

            //check all events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    //populate upcoming events
                    if (ev.startDate >= DateTime.Now && !(ev.startDate.ToString("MMMM dd, yyyy") == DateTime.Now.ToString("MMMM dd, yyyy") && ev.endDate.ToString("MMMM dd, yyyy") == DateTime.Now.ToString("MMMM dd, yyyy")))
                        upcomingEvents.Add(ev);

                    //populate past events
                    else if (ev.endDate < DateTime.Now)
                        pastEvents.Add(ev);

                    //populate active events
                    else
                        activeEvents.Add(ev);
                }

            //setup list sources
            lvActiveEvent.ItemsSource = activeEvents;
            lvUpcomingEvent.ItemsSource = upcomingEvents;

            //empty active events
            if (activeEvents.Count == 0)
            {
                spLVActiveEvent.Height = 60;
                spLVActiveEvent.Margin = new Thickness(0, 10, 0, 0);
                lblActiveEventEmpty.Visibility = Visibility.Visible;
            }

            else
                spLVActiveEvent.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            //empty upcoming events
            if (upcomingEvents.Count == 0)
            {
                spLVUpcomingEvent.Height = 60;
                spLVUpcomingEvent.Margin = new Thickness(0, 10, 0, 0);
                lblUpcomingEventEmpty.Visibility = Visibility.Visible;
            }

            else
                spLVUpcomingEvent.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
        }


        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEventEditCreate));
        }

        private async void btnPopupControl_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            if (btnSender.Name == "btnEnter")
            {
                EnvironmentModel.Event = selectedEvent;
                Frame.Navigate(typeof(PageEventManager));
            }

            else if (btnSender.Name == "btnEdit")
            {
                EnvironmentModel.Event = selectedEvent;
                Frame.Navigate(typeof(PageEventEditCreate));
            }

            else if (btnSender.Name == "btnDelete")
            {
                await Connection.Delete(selectedEvent);
                Frame.Navigate(typeof(PageEvent));
            }

            else if (btnSender.Name == "btnCancel")
            {
                popupEventClick.IsOpen = false;
            }
        }

        private async void Event_ItemClick(object sender, ItemClickEventArgs e)
        {
            //get permissions
            if (Environment.GetEnvironmentVariable("activeUser") == "Guest")
            {
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }

            else
            {
                btnEdit.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
            }
            
            //get event
            selectedEvent = (Event)e.ClickedItem;

            //get info
            lblPopupEventName.Text = selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5);
            lblPopupStartDate.Text = selectedEvent.startDate.ToString("MMMM dd, yyyy");
            lblPopupEndDate.Text = selectedEvent.endDate.ToString("MMMM dd, yyyy");
            Game game = await Connection.Get("Game", selectedEvent.GameID);
            lblPopupGame.Text = $"Game: {game.name}";

            //scroll up to popup
            Grid gridFrame = (Grid)Frame.Parent;
            Grid gridIndex = (Grid)gridFrame.Parent;
            ScrollViewer scroller = (ScrollViewer)gridIndex.Parent;
            scrollerVerticalOffset = scroller.VerticalOffset;
            scroller.ChangeView(scroller.HorizontalOffset, 0, scroller.ZoomFactor);

            //open popup
            popupEventClick.IsOpen = true;
            popupEventClick.Visibility = Visibility.Visible;
        }

        private void popupEventClick_Closed(object sender, object e)
        {
            //scroll back to vertical offset when popup is closed
            Grid gridFrame = (Grid)Frame.Parent;
            Grid gridIndex = (Grid)gridFrame.Parent;
            ScrollViewer scroller = (ScrollViewer)gridIndex.Parent;
            scroller.ChangeView(scroller.HorizontalOffset, scrollerVerticalOffset, scroller.ZoomFactor);
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            //setup past events search
            List<Event> pastEventsSearch = new List<Event>();

            //get filters
            string nameSearch = txtEventSearch.Text.ToLower().Replace(" ", "");
            DateTime startDateSearch = DateTime.MinValue;
            if (dtpStartDateSearch.SelectedDate.HasValue)
                startDateSearch = dtpStartDateSearch.SelectedDate.Value.Date.AddDays(-1);
            DateTime endDateSearch = DateTime.MinValue;
            if (dtpEndDateSearch.SelectedDate.HasValue)
                endDateSearch = dtpEndDateSearch.SelectedDate.Value.Date.AddDays(1);

            //empty filters so get all
            if (string.IsNullOrEmpty(nameSearch) && startDateSearch == DateTime.MinValue && endDateSearch == DateTime.MinValue)
            {
                pastEventsSearch = pastEvents;
            }

            //filter results
            else
            {
                //search date range
                if (startDateSearch != DateTime.MinValue && endDateSearch != DateTime.MinValue)
                {
                    foreach (Event pastEvent in pastEvents)
                        if (pastEvent.startDate >= startDateSearch && pastEvent.endDate <= endDateSearch && pastEvent.name.ToLower().Contains(nameSearch) && !pastEventsSearch.Contains(pastEvent))
                            pastEventsSearch.Add(pastEvent);
                }

                //search start date
                else if (startDateSearch != DateTime.MinValue && endDateSearch == DateTime.MinValue)
                {
                    foreach (Event pastEvent in pastEvents)
                        if (pastEvent.startDate >= startDateSearch && pastEvent.name.ToLower().Contains(nameSearch) && !pastEventsSearch.Contains(pastEvent))
                            pastEventsSearch.Add(pastEvent);
                }

                //search end date
                else if (endDateSearch != DateTime.MinValue && startDateSearch == DateTime.MinValue)
                {
                    foreach (Event pastEvent in pastEvents)
                        if (pastEvent.endDate <= endDateSearch && pastEvent.name.ToLower().Contains(nameSearch) && !pastEventsSearch.Contains(pastEvent))
                            pastEventsSearch.Add(pastEvent);
                }

                //search only name
                if (startDateSearch == DateTime.MinValue && endDateSearch == DateTime.MinValue)
                {
                    foreach (Event pastEvent in pastEvents)
                        if (pastEvent.name.ToLower().Contains(nameSearch) && !pastEventsSearch.Contains(pastEvent))
                            pastEventsSearch.Add(pastEvent);
                }
            }

            //set list source
            lvPastEvent.ItemsSource = pastEventsSearch;

            //empty active events
            if (pastEventsSearch.Count == 0)
            {
                spLVPastEvent.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 221, 221, 221));
                spLVPastEvent.Height = 60;
                spLVPastEvent.Margin = new Thickness(0, 10, 0, 0);
                lblPastEventEmpty.Visibility = Visibility.Visible;
            }

            else
            {
                spLVPastEvent.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
                spLVPastEvent.Height = 180;
                spLVPastEvent.Margin = new Thickness(0, 0, 0, 0);
                lblPastEventEmpty.Visibility = Visibility.Collapsed;
            }
        }
    }
}
