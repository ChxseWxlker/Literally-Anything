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
    public sealed partial class Games : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public Games()
        {
            this.InitializeComponent();
            this.Loaded += Games_Loaded;
        }

        private async void Games_Loaded(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = await Connection.Get("Game");

            foreach (Game g in games)
                if (g.hidden == false)
                {
                    lstQuiz.Items.Add(g.name);
                    visibleGames.Add(g);
                }
        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamesCreate));
        }

        private async void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuiz.SelectedIndex == -1)
                await new MessageDialog("Please choose a quiz to edit").ShowAsync();
            else
                Frame.Navigate(typeof(GamesEdit), visibleGames[lstQuiz.SelectedIndex]);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
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

        private async void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuiz.SelectedIndex == -1)
                await new MessageDialog("Please choose a quiz to delete").ShowAsync();
            else
            {
                //hide game object
                visibleGames[lstQuiz.SelectedIndex].hidden = true;

                //edit game object
                Connection.Update(visibleGames[lstQuiz.SelectedIndex]);

                //reload page
                Frame.Navigate(typeof(Games));
            }
        }
    }
}
