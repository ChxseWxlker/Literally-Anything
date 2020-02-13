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
            EventNameTB.Style = new Style();

            //validation
            if (EventTxt.Text == "")
            {
                EventNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                EventTxt.Focus(FocusState.Keyboard);
                return;
            }
            else if (StartDateDtp.SelectedDate == null)
            {
                await new MessageDialog("enter a date damn").ShowAsync();
                return;
            }

            //navigate back to events
            //Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
    }
}
