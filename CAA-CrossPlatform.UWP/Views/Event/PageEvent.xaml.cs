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
    public sealed partial class PageEvent : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //create lists of events
        List<Event> activeEvents = new List<Event>();
        List<Event> upcomingEvents = new List<Event>();
        List<Event> pastEvents = new List<Event>();

        public PageEvent()
        {
            this.InitializeComponent();
            this.Loaded += PageEvent_Loaded;
        }

        private async void PageEvent_Loaded(object sender, RoutedEventArgs e)
        {
            //stop focus
            //btnLogout.IsEnabled = false;
            //btnLogout.IsEnabled = true;

            //set username
            //txtAccount.Text = Environment.GetEnvironmentVariable("activeUser");

            //get all events
            List<Event> events;
            try
            {
                events = await Connection.Get("Event");
            }
            catch
            {
                events = new List<Event>();
            }

            //check all events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    //populate upcoming events
                    if (ev.startDate > DateTime.Now)
                    {
                        upcomingEvents.Add(ev);
                    }

                    //populate past events
                    else if (ev.endDate < DateTime.Now)
                    {
                        pastEvents.Add(ev);
                    }

                    //populate active events
                    else
                    {
                        activeEvents.Add(ev);
                    }
                }

            if (upcomingEvents == null)
            {
                upcomingEventsLV.Visibility = Visibility.Collapsed;
                //txtNoUpcomingEvents.Visibility = Visibility.Visible;
            }
            else
            {
                upcomingEventsLV.ItemsSource = upcomingEvents;
            }

            if (activeEvents == null)
            {
                activeEventsLV.Visibility = Visibility.Collapsed;
                //txtNoActiveEvents.Visibility = Visibility.Visible;
            }
            else
            {
                activeEventsLV.ItemsSource = activeEvents;
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

        //    private void btnShowPane_Click(object sender, RoutedEventArgs e)
        //    {
        //        svMenu.IsPaneOpen = !svMenu.IsPaneOpen;
        //        if (svMenu.IsPaneOpen)
        //        {
        //            btnShowPane.Content = "\uE00E";
        //            btnEventMenu.Visibility = Visibility.Visible;
        //            btnGameMenu.Visibility = Visibility.Visible;
        //            btnQuestionMenu.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            btnShowPane.Content = "\uE00F";
        //            btnEventMenu.Visibility = Visibility.Collapsed;
        //            btnGameMenu.Visibility = Visibility.Collapsed;
        //            btnQuestionMenu.Visibility = Visibility.Collapsed;
        //        }
        //    }

        //    private void svMenu_PaneClosing(SplitView sender, object args)
        //    {
        //        //hide buttons
        //        btnShowPane.Content = "\uE00F";
        //        btnEventMenu.Visibility = Visibility.Collapsed;
        //        btnGameMenu.Visibility = Visibility.Collapsed;
        //        btnQuestionMenu.Visibility = Visibility.Collapsed;
        //    }

        //    private void btnMenuItem_Click(object sender, RoutedEventArgs e)
        //    {
        //        //get menu button
        //        Button btn = (Button)sender;

        //        //event
        //        if (btn.Content.ToString().Contains("Event"))
        //            Frame.Navigate(typeof(PageEvent));

        //        //game
        //        else if (btn.Content.ToString().Contains("Game"))
        //            Frame.Navigate(typeof(PageGame));

        //        //question
        //        else if (btn.Content.ToString().Contains("Question"))
        //            Frame.Navigate(typeof(PageQuestion));
        //    }

        //private void lbActiveEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    lbUpcomingEvent.SelectedIndex = -1;
        //    lbPastEvent.SelectedIndex = -1;
        //}

        //private void lbUpcomingEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    lbActiveEvent.SelectedIndex = -1;
        //    lbPastEvent.SelectedIndex = -1;
        //}

        //private void lbPastEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    lbActiveEvent.SelectedIndex = -1;
        //    lbUpcomingEvent.SelectedIndex = -1;
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

        private void btnCreateEvent_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEventCreate));
        }

        private void ActiceEvents_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void UpcomingEvents_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pastEvents == null)
            {
                pastEventsLV.Visibility = Visibility.Collapsed;
                //txtNoPastEvents.Visibility = Visibility.Visible;
            }
            else
            {
                pastEventsLV.ItemsSource = pastEvents;
            }
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilterControls.Visibility == Visibility.Visible)
            {
                FilterControls.Visibility = Visibility.Collapsed;
            }
            else
            {
                FilterControls.Visibility = Visibility.Visible;
            }
        }

        private void PastEvents_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
