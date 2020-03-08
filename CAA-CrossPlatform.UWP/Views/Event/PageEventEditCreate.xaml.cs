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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageEventEditCreate : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        //create list of items
        List<Item> items = new List<Item>();

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

            //edit event
            if (selectedEvent.Id != 0)
            {
                //get game
                Game game = await Connection.Get("Game", selectedEvent.GameID);
                cmbGame.SelectedItem = game.name;

                if (!string.IsNullOrEmpty(selectedEvent.name))
                    txtEvent.Text = selectedEvent.displayName.Substring(0, selectedEvent.displayName.Length - 5);

                if (selectedEvent.startDate != null)
                    dtpStartDate.SelectedDate = selectedEvent.startDate;

                if (selectedEvent.endDate != null)
                    dtpEndDate.SelectedDate = selectedEvent.endDate;

                chkMemberOnly.IsChecked = selectedEvent.memberOnly;

                List<EventItem> eventItems = await Connection.Get("EventItem");
                foreach (EventItem eventItem in eventItems)
                    if (eventItem.EventId == selectedEvent.Id)
                    {
                        //get item
                        Item item = await Connection.Get("Item", eventItem.ItemId);

                        //add to list
                        items.Add(item);

                        //populate native textbox first
                        if (txtTrack.Text == "")
                        {
                            txtTrack.Text = item.name;
                            txtTrack.Name = $"txtTrack_{item.Id}";
                        }

                        //create more textboxes
                        else
                        {
                            TextBox txtTrackNew = new TextBox();
                            txtTrackNew.Text = item.name;
                            txtTrackNew.Name = $"txtTrack_{item.Id}";
                            txtTrackNew.HorizontalAlignment = HorizontalAlignment.Left;
                            txtTrackNew.TextWrapping = TextWrapping.Wrap;
                            txtTrackNew.Margin = new Thickness(0, 10, 0, 0);
                            txtTrackNew.FontSize = 25;
                            txtTrackNew.Width = 400;
                            txtTrackNew.TextChanged += txtTrack_TextChanged;

                            //merge to stackpanel
                            spTrackItems.Children.Add(txtTrackNew);
                        }
                    }

                //create empty textbox under
                if (items.Count > 0)
                {
                    TextBox txtTrackEmpty = new TextBox();
                    txtTrackEmpty.Name = "txtTrack";
                    txtTrackEmpty.HorizontalAlignment = HorizontalAlignment.Left;
                    txtTrackEmpty.TextWrapping = TextWrapping.Wrap;
                    txtTrackEmpty.Margin = new Thickness(0, 10, 0, 0);
                    txtTrackEmpty.FontSize = 25;
                    txtTrackEmpty.Width = 400;
                    txtTrackEmpty.TextChanged += txtTrack_TextChanged;

                    //merge to stackpanel
                    spTrackItems.Children.Add(txtTrackEmpty);
                }
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //validate name
            if (txtEvent.Text == "")
            {
                txtEvent.Focus(FocusState.Keyboard);
                return;
            }

            //validate start date
            else if (dtpStartDate.SelectedDate == null)
            {
                dtpStartDate.Focus(FocusState.Keyboard);
                return;
            }

            //validate end date
            else if (dtpEndDate.SelectedDate == null)
            {
                dtpEndDate.Focus(FocusState.Keyboard);
                return;
            }

            //validate date range
            else if (dtpEndDate.SelectedDate < dtpStartDate.SelectedDate)
            {
                dtpEndDate.Focus(FocusState.Keyboard);
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
                abbreviation += $"{dtpStartDate.SelectedDate.Value.DateTime.Month.ToString("00")}{dtpStartDate.SelectedDate.Value.DateTime.Year}";

                //event exists and is visible
                if (ev.nameAbbrev == abbreviation && ev.hidden == false && selectedEvent.Id == 0)
                {
                    txtEvent.Focus(FocusState.Keyboard);
                    await new MessageDialog("That event already exists, enter a different name or date.").ShowAsync();
                    return;
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
            newEvent.startDate = dtpStartDate.SelectedDate.Value.DateTime;
            newEvent.endDate = dtpEndDate.SelectedDate.Value.DateTime;
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

            //save to database
            if (selectedEvent.Id != 0)
            {
                newEvent.Id = selectedEvent.Id;
                await Connection.Update(newEvent);
            }

            else if (selectedEvent.Id == 0)
                newEvent.Id = await Connection.Insert(newEvent);

            //create trackable items
            if (newEvent.Id != -1)
            {
                foreach (TextBox txtItem in spTrackItems.Children)
                {
                    if (!string.IsNullOrEmpty(txtItem.Text))
                    {
                        //update
                        if (txtItem.Name != "txtTrack")
                        {
                            int id = Convert.ToInt32(txtItem.Name.Substring(txtItem.Name.IndexOf('_') + 1));
                            Item item = await Connection.Get("Item", id);
                            item.name = txtItem.Text;
                            await Connection.Update(item);
                        }

                        //create
                        else if (txtItem.Name == "txtTrack")
                        {
                            //create item
                            Item item = new Item();
                            item.name = txtItem.Text;
                            item.valueType = "int";
                            item.Id = await Connection.Insert(item);

                            //create even item
                            EventItem eventItem = new EventItem();
                            eventItem.EventId = newEvent.Id;
                            eventItem.ItemId = item.Id;
                            eventItem.Id = await Connection.Insert(eventItem);
                        }
                    }
                }
            }

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

            if (!string.IsNullOrEmpty(txtEvent.Text))
                newEvent.name = txtEvent.Text;

            if (dtpStartDate.SelectedDate != null)
                newEvent.startDate = dtpStartDate.SelectedDate.Value.DateTime;

            if (dtpEndDate.SelectedDate != null)
                newEvent.endDate = dtpEndDate.SelectedDate.Value.DateTime;

            newEvent.memberOnly = chkMemberOnly.IsChecked ?? false;

            //store event for return
            EnvironmentModel.Event = newEvent;

            //navigate to create a game
            Frame.Navigate(typeof(PageGameEditCreate));
        }

        private async void txtTrack_TextChanged(object sender, TextChangedEventArgs e)
        {
            //get textbox
            TextBox txtSender = (TextBox)sender;

            //create next trackable item
            if (spTrackItems.Children.IndexOf(txtSender) == spTrackItems.Children.Count - 1)
            {
                if (!string.IsNullOrEmpty(txtSender.Text))
                {
                    TextBox txtTrackNew = new TextBox();
                    txtTrackNew.Name = "txtTrack";
                    txtTrackNew.HorizontalAlignment = HorizontalAlignment.Left;
                    txtTrackNew.TextWrapping = TextWrapping.Wrap;
                    txtTrackNew.Margin = new Thickness(0, 10, 0, 0);
                    txtTrackNew.FontSize = 25;
                    txtTrackNew.Width = 400;
                    txtTrackNew.TextChanged += txtTrack_TextChanged;

                    //merge to stackpanel
                    spTrackItems.Children.Add(txtTrackNew);
                }
            }

            //remove trackable item
            else if (string.IsNullOrEmpty(txtSender.Text) && spTrackItems.Children.Count > 1)
            {
                //focus other track item
                try
                {
                    TextBox txtFocus = (TextBox)spTrackItems.Children[spTrackItems.Children.IndexOf(txtSender) - 1];
                    txtFocus.Focus(FocusState.Keyboard);
                }
                catch
                {
                    TextBox txtFocus = (TextBox)spTrackItems.Children[1];
                    txtFocus.Focus(FocusState.Keyboard);
                }

                //delete from database
                if (txtSender.Name != "txtTrack")
                {
                    int id = Convert.ToInt32(txtSender.Name.Substring(txtSender.Name.IndexOf('_') + 1));
                    Item item = await Connection.Get("Item", id);
                    deleteItemsQueue.Add(item);

                    //remove textbox from stackpanel
                    spTrackItems.Children.Remove(txtSender);
                }

                //remove textbox from stackpanel
                else
                    spTrackItems.Children.Remove(txtSender);
            }
        }
    }
}