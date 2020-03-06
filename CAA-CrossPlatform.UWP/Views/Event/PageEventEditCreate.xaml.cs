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
        //setup api
        static ApiHandler api = new ApiHandler();

        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        //create list of items
        List<Item> items = new List<Item>();

        //setup selected event
        Event selectedEvent = null;

        public PageEventEditCreate()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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
            if (PassItem.environmentEvent != null)
            {
                //get event
                selectedEvent = PassItem.environmentEvent;

                //reset environment event
                PassItem.environmentEvent = null;

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

            base.OnNavigatedTo(e);
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //reset validation template
            lblEventName.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            lblStartDate.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            lblEndDate.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];

            //validate name
            if (txtEvent.Text == "")
            {
                lblEventName.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                txtEvent.Focus(FocusState.Keyboard);
                return;
            }

            //validate start date
            else if (dtpStartDate.SelectedDate == null)
            {
                lblStartDate.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                dtpStartDate.Focus(FocusState.Keyboard);
                return;
            }

            //validate end date
            else if (dtpEndDate.SelectedDate == null)
            {
                lblEndDate.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                dtpEndDate.Focus(FocusState.Keyboard);
                return;
            }

            //validate date range
            else if (dtpEndDate.SelectedDate < dtpStartDate.SelectedDate)
            {
                lblStartDate.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                lblEndDate.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
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
                if (ev.nameAbbrev == abbreviation && ev.hidden == false && selectedEvent.Id != ev.Id)
                {
                    lblEventName.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    txtEvent.Focus(FocusState.Keyboard);
                    await new MessageDialog("That event already exists, please enter a different name or date").ShowAsync();
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
                        Connection.Update(ev);
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
            if (selectedEvent != null)
                newEvent.Id = selectedEvent.Id;
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
            if (selectedEvent == null)
                newEvent.Id = await Connection.Insert(newEvent);
            else
                Connection.Update(newEvent);

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
                            Connection.Update(item);
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

            //navigate away
            Frame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void btnCreateGame_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEvent.Text))
                selectedEvent.name = txtEvent.Text;

            if (dtpStartDate.SelectedDate != null)
                selectedEvent.startDate = dtpStartDate.SelectedDate.Value.DateTime;

            if (dtpEndDate.SelectedDate != null)
                selectedEvent.endDate = dtpEndDate.SelectedDate.Value.DateTime;

            selectedEvent.memberOnly = chkMemberOnly.IsChecked ?? false;

            //store event for return
            PassItem.environmentEvent = selectedEvent;

            //navigate to create a game
            Frame.Navigate(typeof(PageGameCreate));
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
            if (string.IsNullOrEmpty(txtSender.Text) && spTrackItems.Children.Count > 1)
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
                    try
                    {
                        int id = Convert.ToInt32(txtSender.Name.Substring(txtSender.Name.IndexOf('_') + 1));
                        Item item = await Connection.Get("Item", id);

                        List<EventItem> eventItems = await Connection.Get("EventItem");
                        EventItem selectedEventItem = null;
                        foreach (EventItem eventItem in eventItems)
                            if (eventItem.EventId == selectedEvent.Id && eventItem.ItemId == id)
                                selectedEventItem = eventItem;

                        if (selectedEventItem != null)
                            await Connection.Delete(selectedEventItem);

                        await Connection.Delete(item);

                        //remove textbox from stackpanel
                        spTrackItems.Children.Remove(txtSender);
                    }
                    catch
                    {
                        await new MessageDialog("Unable to remove tracking item").ShowAsync();
                    }

                }

                //remove textbox from stackpanel
                else
                    spTrackItems.Children.Remove(txtSender);
            }
        }
    }
}