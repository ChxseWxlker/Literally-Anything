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
    public sealed partial class PageEventCreate : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public PageEventCreate()
        {
            this.InitializeComponent();
            this.Loaded += EventsCreate_Loaded;
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var selections = (Dictionary<string, string>)e.Parameter;
                EventTxt.Text = selections["name"];

                if (selections["startDate"] != "")
                    StartDateDtp.SelectedDate = Convert.ToDateTime(selections["startDate"]);

                if (selections["endDate"] != "")
                EndDateDtp.SelectedDate = Convert.ToDateTime(selections["endDate"]);

                if (selections["memberOnly"] != "")
                    MemberOnlyChk.IsChecked = Convert.ToBoolean(selections["memberOnly"]);
            }

            base.OnNavigatedTo(e);
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private async void EventsCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get all games
            List<Game> games = await Connection.Get("Game");

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.name);
                    visibleGames.Add(game);
                }
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //reset validation template
            EventNameTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            StartDateTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];
            EndDateTB.Style = (Style)Application.Current.Resources["ValidationResetTemplate"];

            //validate name
            if (EventTxt.Text == "")
            {
                EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EventTxt.Focus(FocusState.Keyboard);
                return;
            }

            //validate start date
            else if (StartDateDtp.SelectedDate == null)
            {
                StartDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                StartDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //validate end date
            else if (EndDateDtp.SelectedDate == null)
            {
                EndDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //validate date range
            else if (EndDateDtp.SelectedDate <= StartDateDtp.SelectedDate)
            {
                StartDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EndDateDtp.Focus(FocusState.Keyboard);
                return;
            }

            //validate abbreviated name
            List<Event> events = await Connection.Get("Event");
            foreach (Event ev2 in events)
            {
                //get abbreviation
                string abbreviation = "";
                foreach (string word in EventTxt.Text.Replace("'", "''").Split(' '))
                {
                    char[] letters = word.ToCharArray();
                    abbreviation += char.ToUpper(letters[0]);
                }
                abbreviation += $"{StartDateDtp.SelectedDate.Value.DateTime.Month.ToString("00")}{StartDateDtp.SelectedDate.Value.DateTime.Year}";

                //event exists and is visible
                if (ev2.nameAbbrev == abbreviation && ev2.hidden == false)
                {
                    EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That event already exists, please enter a different name or date").ShowAsync();
                    return;
                }

                //event exists but is hidden
                else if (ev2.nameAbbrev == abbreviation && ev2.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That event is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        ev2.hidden = false;
                        Connection.Update(ev2);
                        Frame.Navigate(Frame.BackStack.Last().SourcePageType);
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //fix special characters for sql
            string eventName = EventTxt.Text.Replace("'", "''");

            //setup event record
            Event ev = new Event();
            ev.startDate = StartDateDtp.SelectedDate.Value.DateTime;
            ev.endDate = EndDateDtp.SelectedDate.Value.DateTime;
            ev.displayName = $"{eventName} {ev.startDate.Year}";
            ev.name = ev.displayName.Replace(" ", "");
            ev.nameAbbrev = "";
            foreach (string word in eventName.Split(' '))
            {
                char[] letters = word.ToCharArray();
                ev.nameAbbrev += char.ToUpper(letters[0]);
            }
            ev.nameAbbrev += $"{ev.startDate.Month.ToString("00")}{ev.startDate.Year}";
            ev.memberOnly = MemberOnlyChk.IsChecked ?? false;
            ev.GameID = visibleGames[QuizCmb.SelectedIndex].Id;

            //save to database
            ev.Id = await Connection.Insert(ev);

            //navigate away if successful
            if (ev.Id != -1)
                Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void btnCreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            //setup object with parameters to send
            var selections = new Dictionary<string, string>
            {
                { "name", EventTxt.Text },
                { "startDate", StartDateDtp.SelectedDate.ToString() },
                { "endDate", EndDateDtp.SelectedDate.ToString() },
                { "memberOnly", MemberOnlyChk.IsChecked.ToString() }
            };
            Frame.Navigate(typeof(PageGameCreate), selections);
        }
    }
}