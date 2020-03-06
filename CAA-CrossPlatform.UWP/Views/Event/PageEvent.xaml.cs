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
using CAA_CrossPlatform.UWP.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageEvent : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //create lists of events
        List<Event> activeEvents = new List<Event>();
        List<Event> upcomingEvents = new List<Event>();
        List<Event> pastEvents = new List<Event>();

        //create selected event
        Event selectedEvent;

        public PageEvent()
        {
            this.InitializeComponent();
            this.Loaded += PageEvent_Loaded;
        }

        private async void PageEvent_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = await Connection.Get("Event");

            //check all events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    //populate upcoming events
                    if (ev.startDate > DateTime.Now)
                        upcomingEvents.Add(ev);

                    //populate past events
                    else if (ev.endDate < DateTime.Now)
                        pastEvents.Add(ev);

                    //populate active events
                    else
                        activeEvents.Add(ev);
                }

            //setup list sources
            activeEventsLV.ItemsSource = activeEvents;
            upcomingEventsLV.ItemsSource = upcomingEvents;

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

        private async void Event_ItemClick(object sender, ItemClickEventArgs e)
        {
            //get permissions
            if (Environment.GetEnvironmentVariable("activeUser") == "Guest")
            {
                btnCreateEvent.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
            }

            else
            {
                btnCreateEvent.Visibility = Visibility.Visible;
                btnEdit.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
            }
            
            //get event
            selectedEvent = (Event)e.ClickedItem;

            //get info
            lblPopupEventName.Text = $"Event: {selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5)}";
            lblPopupStartDate.Text = $"Start Date: {selectedEvent.startDate.ToString("MMMM dd, yyyy")}";
            lblPopupEndDate.Text = $"End Date: {selectedEvent.endDate.ToString("MMMM dd, yyyy")}";
            Game game = await Connection.Get("Game", selectedEvent.GameID);
            lblPopupGame.Text = $"Game: {game.name}";

            popupEventClick.IsOpen = true;
            popupEventClick.Visibility = Visibility.Visible;
        }

        private async void btnPopupControl_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            if (btnSender.Name == "btnEnter")
            {
                PassItem.environmentEvent = selectedEvent;
                Frame.Navigate(typeof(PageEventManager));
            }

            else if (btnSender.Name == "btnEdit")
            {
                PassItem.environmentEvent = selectedEvent;
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

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pastEvents.Count == 0)
            {
                pastEventsLV.Visibility = Visibility.Collapsed;
                //txtNoPastEvents.Visibility = Visibility.Visible;
            }
            else
            {
                pastEventsLV.ItemsSource = pastEvents;
            }
        }

        private void btnFilterDropdown_Click(object sender, RoutedEventArgs e)
        {
            if (FilterControls.Visibility == Visibility.Visible)
            {
                btnFilterDropdown.Content = "\uE70D";
                FilterControls.Visibility = Visibility.Collapsed;
                lblNoResult.Visibility = Visibility.Visible;
            }
            else
            {
                btnFilterDropdown.Content = "\uE70E";
                FilterControls.Visibility = Visibility.Visible;
            }
        }

        //private async void btnLogout_Click(object sender, RoutedEventArgs e)
        //{
        //    //prompt user
        //    ContentDialog logoutDialog = new ContentDialog
        //    {
        //        Title = "Logout?",
        //        Content = "You will be redirected to the home page and locked out until you log back in. Are you sure you want to logout?",
        //        PrimaryButtonText = "Logout",
        //        CloseButtonText = "Cancel"
        //    };

        //    ContentDialogResult logoutRes = await logoutDialog.ShowAsync();

        //    //log user out
        //    if (logoutRes == ContentDialogResult.Primary)
        //    {
        //        //reset active username
        //        Environment.SetEnvironmentVariable("activeUser", "");

        //        //update menu
        //        //txtAccount.Text = "";

        //        //logout
        //        api.Logout();

        //        //redirect to index
        //        Frame.Navigate(typeof(PageIndex));
        //    }
        //}

        

       

        //private async void btnActiveEvent_Click(object sender, RoutedEventArgs e)
        //{
        //    //check
        //    if (lbActiveEvent.SelectedIndex == -1)
        //    {
        //        await new MessageDialog("Please choose an active event.").ShowAsync();
        //        return;
        //    }

        //    //get sender
        //    Button btn = (Button)sender;

        //    //manage event
        //    if (btn.Name.Contains("Manage"))
        //        Frame.Navigate(typeof(PageEventManager), activeEvents[lbActiveEvent.SelectedIndex]);

        //    //view event
        //    else if (btn.Name.Contains("View"))
        //    {
        //        //TODO: create popup with info
        //    }

        //    //edit event
        //    else if (btn.Name.Contains("Edit"))
        //        Frame.Navigate(typeof(PageEventEdit), activeEvents[lbActiveEvent.SelectedIndex]);

        //    //delete event
        //    else if (btn.Name.Contains("Delete"))
        //    {
        //        Connection.Delete(activeEvents[lbActiveEvent.SelectedIndex]);
        //        Frame.Navigate(typeof(PageEvent));
        //    }
        //}

        //private async void btnUpcomingEvent_Click(object sender, RoutedEventArgs e)
        //{
        //    //check
        //    if (lbUpcomingEvent.SelectedIndex == -1)
        //    {
        //        await new MessageDialog("Please choose an upcoming event.").ShowAsync();
        //        return;
        //    }

        //    //get sender
        //    Button btn = (Button)sender;

        //    //manage event
        //    if (btn.Name.Contains("Manage"))
        //        Frame.Navigate(typeof(PageEventManager), upcomingEvents[lbUpcomingEvent.SelectedIndex]);

        //    //view event
        //    else if (btn.Name.Contains("View"))
        //    {
        //        //TODO: create popup with info
        //    }

        //    //edit event
        //    else if (btn.Name.Contains("Edit"))
        //        Frame.Navigate(typeof(PageEventEdit), upcomingEvents[lbUpcomingEvent.SelectedIndex]);

        //    //delete event
        //    else if (btn.Name.Contains("Delete"))
        //    {
        //        Connection.Delete(upcomingEvents[lbUpcomingEvent.SelectedIndex]);
        //        Frame.Navigate(typeof(PageEvent));
        //    }
        //}

        //private async void btnPastEvent_Click(object sender, RoutedEventArgs e)
        //{
        //    //check
        //    if (lbPastEvent.SelectedIndex == -1)
        //    {
        //        await new MessageDialog("Please choose a past event.").ShowAsync();
        //        return;
        //    }

        //    //get sender
        //    Button btn = (Button)sender;

        //    //manage event
        //    if (btn.Name.Contains("Manage"))
        //        Frame.Navigate(typeof(PageEventManager), pastEvents[lbPastEvent.SelectedIndex]);

        //    //view event
        //    else if (btn.Name.Contains("View"))
        //    {
        //        //TODO: create popup with info
        //    }

        //    //edit event
        //    else if (btn.Name.Contains("Edit"))
        //        Frame.Navigate(typeof(PageEventEdit), pastEvents[lbPastEvent.SelectedIndex]);

        //    //delete event
        //    else if (btn.Name.Contains("Delete"))
        //    {
        //        Connection.Delete(pastEvents[lbPastEvent.SelectedIndex]);
        //        Frame.Navigate(typeof(PageEvent));
        //    }
        //}
    }
}
