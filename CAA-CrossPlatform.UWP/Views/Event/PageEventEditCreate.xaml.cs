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
                foreach (EventItem eventItem in eventItems)
                    if (eventItem.EventId == selectedEvent.Id)
                    {
                        //get item
                        Item item = await Connection.Get("Item", eventItem.ItemId);

                        //add to list
                        items.Add(item);
                        lbItem.SelectedItems.Add(item.name);
                    }
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //validate name required
            if (string.IsNullOrEmpty(txtEvent.Text))
            {
                txtEvent.Focus(FocusState.Keyboard);
                await new MessageDialog("Event name is required.").ShowAsync();
                return;
            }

            //validate start date
            if (!dpStart.SelectedDate.HasValue)
            {
                dpStart.Focus(FocusState.Keyboard);
                await new MessageDialog("Start date is required.").ShowAsync();
                return;
            }

            //validate start time
            if (!tpStart.SelectedTime.HasValue)
            {
                tpStart.Focus(FocusState.Keyboard);
                await new MessageDialog("Start time is required.").ShowAsync();
                return;
            }

            //validate end date
            if (dpEnd.SelectedDate == null)
            {
                dpEnd.Focus(FocusState.Keyboard);
                await new MessageDialog("End date is required.").ShowAsync();
                return;
            }

            if (!tpEnd.SelectedTime.HasValue)
            {
                tpEnd.Focus(FocusState.Keyboard);
                await new MessageDialog("End time is required.").ShowAsync();
                return;
            }

            //validate date range
            if (dpEnd.SelectedDate < dpStart.SelectedDate)
            {
                dpEnd.Focus(FocusState.Keyboard);
                await new MessageDialog("End date must be after start date.").ShowAsync();
                return;
            }

            //validate game
            if (cmbGame.SelectedIndex == -1)
            {
                cmbGame.Focus(FocusState.Keyboard);
                await new MessageDialog("A game is required.").ShowAsync();
                return;
            }

            //validate abbreviated name
            List<Event> events = await Connection.Get("Event");
            foreach (Event ev in events)
            {
                //get abbreviation
                string abbreviation = "";
                foreach (string word in txtEvent.Text.Replace("'", "''").Split(' '))
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
                        await new MessageDialog("That event already exists, enter a different name or date.").ShowAsync();
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

            //fix special characters for sql
            string eventName = txtEvent.Text.Replace("'", "''");

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

            ////create trackable items
            //if (newEvent.Id != -1)
            //{
            //    foreach (TextBox txtItem in spTrackItems.Children)
            //    {
            //        if (!string.IsNullOrEmpty(txtItem.Text))
            //        {
            //            //update
            //            if (txtItem.Name != "txtTrack")
            //            {
            //                int id = Convert.ToInt32(txtItem.Name.Substring(txtItem.Name.IndexOf('_') + 1));
            //                Item item = await Connection.Get("Item", id);
            //                item.name = txtItem.Text;
            //                await Connection.Update(item);
            //            }

            //            //create
            //            else if (txtItem.Name == "txtTrack")
            //            {
            //                //create item
            //                Item item = new Item();
            //                item.name = txtItem.Text;
            //                item.valueType = "int";
            //                item.Id = await Connection.Insert(item);

            //                //create even item
            //                EventItem eventItem = new EventItem();
            //                eventItem.EventId = newEvent.Id;
            //                eventItem.ItemId = item.Id;
            //                eventItem.Id = await Connection.Insert(eventItem);
            //            }
            //        }
            //    }
            //}

            //delete events
            List<EventItem> eventItems = await Connection.Get("EventItem");
            foreach (Item item in deleteItemsQueue)
            {
                //delete relationship
                foreach (EventItem eventItem in eventItems)
                    if (eventItem.EventId == selectedEvent.Id && eventItem.ItemId == item.Id)
                        await Connection.Delete(eventItem);
                
                //delete item
                await Connection.Delete(item);
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