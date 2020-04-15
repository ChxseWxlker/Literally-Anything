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
using CAA_CrossPlatform.UWP.Models;
using System.Text.RegularExpressions;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageEventEditCreate : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        //create list of items
        List<Item> visibleItems = new List<Item>();

        //setup selected event
        Event selectedEvent = new Event();

        //setup items to be deleted
        List<Item> deleteItemsQueue = new List<Item>();

        public PageEventEditCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageEventEditCreate_Loaded;
        }

        private async void PageEventEditCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get event
            selectedEvent = EnvironmentModel.Event;
            EnvironmentModel.Event = new Event();

            //get all games
            List<Game> games = await Connection.Get("Game");

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    cmbGame.Items.Add(game.name);
                    visibleGames.Add(game);
                }

            //populate listbox
            List<Item> items = await Connection.Get("Item");
            foreach (Item item in items)
                if (item.hidden == false)
                {
                    lbItem.Items.Add(item.name);
                    visibleItems.Add(item);
                }

            //set properties
            if (selectedEvent.Id != 0)
            {
                //setup button
                if (selectedEvent.Id != -1)
                    btnSubmit.Content = "Save";

                //set properties
                try
                {
                    Game game = await Connection.Get("Game", selectedEvent.GameID);
                    cmbGame.SelectedItem = game.name;
                }
                catch { }

                if (!string.IsNullOrEmpty(selectedEvent.displayName))
                    txtEvent.Text = selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5);

                if (selectedEvent.startDate != null)
                {
                    dpStart.SelectedDate = selectedEvent.startDate.Date;
                    tpStart.SelectedTime = selectedEvent.startDate.TimeOfDay;
                }

                if (selectedEvent.endDate != null)
                {
                    dpEnd.SelectedDate = selectedEvent.endDate.Date;
                    tpEnd.SelectedTime = selectedEvent.endDate.TimeOfDay;
                }

                chkMemberOnly.IsChecked = selectedEvent.memberOnly;

                List<EventItem> eventItems = await Connection.Get("EventItem");
                foreach (Item item in visibleItems)
                    foreach (EventItem eventItem in eventItems)
                        if (eventItem.EventId == selectedEvent.Id && eventItem.ItemId == item.Id)
                            lbItem.SelectedItems.Add(item.name);
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //validate name required
            if (string.IsNullOrEmpty(txtEvent.Text))
            {
                txtEvent.Focus(FocusState.Keyboard);
                PageIndex.ShowError("Event name is required.");
                return;
            }

            //validate start date
            if (!dpStart.SelectedDate.HasValue)
            {
                dpStart.Focus(FocusState.Keyboard);
                PageIndex.ShowError("Start date is required.");
                return;
            }

            //validate start time
            if (!tpStart.SelectedTime.HasValue)
            {
                tpStart.Focus(FocusState.Keyboard);
                PageIndex.ShowError("Start time is required.");
                return;
            }

            //validate end date
            if (dpEnd.SelectedDate == null)
            {
                dpEnd.Focus(FocusState.Keyboard);
                PageIndex.ShowError("End date is required.");
                return;
            }

            if (!tpEnd.SelectedTime.HasValue)
            {
                tpEnd.Focus(FocusState.Keyboard);
                PageIndex.ShowError("End time is required.");
                return;
            }

            //validate date range
            if (dpEnd.SelectedDate < dpStart.SelectedDate)
            {
                dpEnd.Focus(FocusState.Keyboard);
                PageIndex.ShowError("End date must be after start date.");
                return;
            }

            //validate game
            if (cmbGame.SelectedIndex == -1)
            {
                cmbGame.Focus(FocusState.Keyboard);
                PageIndex.ShowError("A game is required.");
                return;
            }

            //delete previous trackable items if possible
            List<EventItem> eventItemDeleteQueue = new List<EventItem>();
            if (selectedEvent.Id != 0 && selectedEvent.Id != -1)
            {
                List<EventItem> eventItems = await Connection.Get("EventItem");
                List<AttendanceItem> attendanceItems = await Connection.Get("AttendanceItem");
                foreach (EventItem eventItem in eventItems)
                    if (eventItem.EventId == selectedEvent.Id)
                    {
                        //get item
                        Item item = await Connection.Get("Item", eventItem.ItemId);
                        int attendanceCount = 0;

                        //check if trying to delete connection
                        if (!lbItem.SelectedItems.Contains(item.name))
                            foreach (AttendanceItem attendanceItem in attendanceItems)
                                if (attendanceItem.EventItemId == eventItem.Id)
                                    attendanceCount++;

                        //delete connection if no tracking data
                        if (attendanceCount == 0)
                            eventItemDeleteQueue.Add(eventItem);

                        else
                        {
                            lbItem.SelectedItems.Add(item.name);
                            PageIndex.ShowError($"Cannot delete {item.name} item, it is tracking data for this event.");
                            return;
                        }
                    }

                //delete event items in queue
                foreach (EventItem eventItem in eventItemDeleteQueue)
                    await Connection.Delete(eventItem);
            }

            //validate abbreviated name
            List<Event> events = await Connection.Get("Event");
            foreach (Event ev in events)
            {
                //get abbreviation
                string abbreviation = "";
                foreach (string word in txtEvent.Text.Split(' '))
                {
                    char[] letters = word.ToCharArray();
                    abbreviation += char.ToUpper(letters[0]);
                }
                abbreviation += $"{dpStart.SelectedDate.Value.Date.Month.ToString("00")}{dpStart.SelectedDate.Value.Date.Year}";

                //event exists and is visible
                if (ev.nameAbbrev == abbreviation && ev.hidden == false)
                {
                    if (selectedEvent.Id == 0 || selectedEvent.Id == -1)
                    {
                        txtEvent.Focus(FocusState.Keyboard);
                        PageIndex.ShowError("That event already exists, enter a different name or date.");
                        return;
                    }
                }

                //event exists but is hidden
                else if (ev.nameAbbrev == abbreviation && ev.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That event is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        ev.hidden = false;
                        await Connection.Update(ev);
                        Frame.Navigate(Frame.BackStack.Last().SourcePageType);
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //get event name
            string eventName = txtEvent.Text;

            //setup event record
            Event newEvent = new Event();
            newEvent.startDate = dpStart.SelectedDate.Value.Date.Add(tpStart.SelectedTime.Value);
            newEvent.endDate = dpEnd.SelectedDate.Value.Date.Add(tpEnd.SelectedTime.Value);
            newEvent.displayName = $"{eventName} {newEvent.startDate.Year}";
            newEvent.name = newEvent.displayName.Replace(" ", "");
            newEvent.nameAbbrev = "";
            foreach (string word in eventName.Split(' '))
            {
                char[] letters = word.ToCharArray();
                newEvent.nameAbbrev += char.ToUpper(letters[0]);
            }
            newEvent.nameAbbrev += $"{newEvent.startDate.Month.ToString("00")}{newEvent.startDate.Year}";
            newEvent.memberOnly = chkMemberOnly.IsChecked ?? false;
            newEvent.GameID = visibleGames[cmbGame.SelectedIndex].Id;

            if (selectedEvent.Id == 0 || selectedEvent.Id == -1)
                newEvent.Id = await Connection.Insert(newEvent);

            else
            {
                newEvent.Id = selectedEvent.Id;
                await Connection.Update(newEvent);
            }

            if (newEvent.Id != -1)
            {
                //create trackable items
                foreach (Item item in visibleItems)
                    if (lbItem.SelectedItems.Contains(item.name))
                    {
                        EventItem eventItem = new EventItem();
                        eventItem.EventId = newEvent.Id;
                        eventItem.ItemId = item.Id;
                        eventItem.Id = await Connection.Insert(eventItem);
                    }
            }

            //navigate away
            Frame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void btnCreateGame_Click(object sender, RoutedEventArgs e)
        {
            Event newEvent = new Event();
            newEvent.Id = -1;

            if (selectedEvent.Id != -1 && selectedEvent.Id != 0)
                newEvent.Id = selectedEvent.Id;

            if (dpStart.SelectedDate != null)
                newEvent.startDate = dpStart.SelectedDate.Value.Date.Add(tpStart.SelectedTime.Value);

            if (dpEnd.SelectedDate != null)
                newEvent.endDate = dpEnd.SelectedDate.Value.Date.Add(tpEnd.SelectedTime.Value);

            if (!string.IsNullOrEmpty(txtEvent.Text))
                newEvent.displayName = $"{txtEvent.Text} {newEvent.startDate.Year}";

            if (cmbGame.SelectedIndex != -1)
                newEvent.GameID = visibleGames[cmbGame.SelectedIndex].Id;

            newEvent.memberOnly = chkMemberOnly.IsChecked ?? false;

            //store event for return
            EnvironmentModel.Event = newEvent;

            //navigate to create a game
            Frame.Navigate(typeof(PageGameEditCreate));
        }

        private void btnCreateItem_Click(object sender, RoutedEventArgs e)
        {
            Event newEvent = new Event();
            newEvent.Id = -1;

            if (selectedEvent.Id != -1 && selectedEvent.Id != 0)
                newEvent.Id = selectedEvent.Id;

            if (dpStart.SelectedDate != null)
                newEvent.startDate = dpStart.SelectedDate.Value.Date.Add(tpStart.SelectedTime.Value);

            if (dpEnd.SelectedDate != null)
                newEvent.endDate = dpEnd.SelectedDate.Value.Date.Add(tpEnd.SelectedTime.Value);

            if (!string.IsNullOrEmpty(txtEvent.Text))
                newEvent.displayName = $"{txtEvent.Text} {newEvent.startDate.Year}";

            if (cmbGame.SelectedIndex != -1)
                newEvent.GameID = visibleGames[cmbGame.SelectedIndex].Id;

            newEvent.memberOnly = chkMemberOnly.IsChecked ?? false;

            //store event for return
            EnvironmentModel.Event = newEvent;

            //navigate to create a game
            Frame.Navigate(typeof(PageItemEditCreate));
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower().Replace(" ", "");
            lbItem.Items.Clear();
            SolidColorBrush btnBg = (SolidColorBrush)btnSearch.Background;

            //change to clear
            if (btnBg.Color.G.ToString() == "82")
            {
                foreach (Item item in visibleItems)
                    if (item.name.ToLower().Replace(" ", "").Contains(search))
                        lbItem.Items.Add(item.name);

                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplateRed"];
                btnSearch.Content = "\uE894";
            }

            //change to search
            else if (btnBg.Color.G.ToString() == "14")
            {
                foreach (Item item in visibleItems)
                    lbItem.Items.Add(item.name);

                txtSearch.Text = "";
                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplate"];
                btnSearch.Content = "\uE1A3";
            }
        }
    }
}