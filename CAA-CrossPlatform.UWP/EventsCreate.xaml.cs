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
    public sealed partial class EventsCreate : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public EventsCreate()
        {
            this.InitializeComponent();
            this.Loaded += EventsCreate_Loaded;
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

        private void EventsCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get all games
            List<Game> games = Json.Read("game.json");

            //populate listbox
            foreach (Game game in games)
                if (game.hidden == false)
                {
                    QuizCmb.Items.Add(game.title);
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

            //fix special characters for sql
            string eventName = EventTxt.Text.Replace("'", "''");

            //setup event record
            Event ev = new Event();
            ev.startDate = StartDateDtp.SelectedDate.Value.UtcDateTime;
            ev.endDate = EndDateDtp.SelectedDate.Value.UtcDateTime;
            ev.displayName = $"{eventName} {ev.startDate.Year}";
            ev.name = eventName.Replace(" ", "") + ev.startDate.Year;
            ev.nameAbbrev = "";
            foreach (string word in eventName.Split(' '))
            {
                char[] letters = word.ToCharArray();
                ev.nameAbbrev += char.ToUpper(letters[0]);
            }
            ev.nameAbbrev += $"{ev.startDate.Month.ToString("00")}{ev.startDate.Year}";
            ev.memberOnly = MemberOnlyChk.IsChecked ?? false;

            //save to database
            ev.Id = await Connection.Insert(ev);
            
            //navigate away if successful
            if (ev.Id != -1)
                Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private async void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Game g = new Game();
            g.title = "Test";
            g.EventID = 2;
            g.Id = await Connection.Insert(g);

            Question q = new Question();
            q.name = "Test";
            q.GameID = g.Id;
            q.Id = await Connection.Insert(q);

            Answer a1 = new Answer();
            a1.name = "testWrong";
            a1.correct = false;
            a1.QuestionID = q.Id;
            a1.Id = await Connection.Insert(a1);

            Answer a2 = new Answer();
            a2.name = "testWrong";
            a2.correct = false;
            a2.QuestionID = q.Id;
            a2.Id = await Connection.Insert(a2);

            Answer a3 = new Answer();
            a3.name = "testRight";
            a3.correct = true;
            a3.QuestionID = q.Id;
            a3.Id = await Connection.Insert(a3);

            //Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
    }
}
