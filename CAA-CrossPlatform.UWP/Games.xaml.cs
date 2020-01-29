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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class Games : Page
    {
        List<Game> listGames = new List<Game>();

        public Games()
        {
            this.InitializeComponent();
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
                Frame.Navigate(typeof(GamesEdit), listGames[lstQuiz.SelectedIndex]);
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = Json.Read("game.json");

            foreach (Game g in games)
                if (g.hidden == false)
                {
                    lstQuiz.Items.Add(g.title);
                    listGames.Add(g);
                }
        }

        private async void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuiz.SelectedIndex == -1)
                await new MessageDialog("Please choose a quiz to delete").ShowAsync();
            else
            {
                //hide game object
                listGames[lstQuiz.SelectedIndex].hidden = true;

                //edit game object
                Json.Edit(listGames[lstQuiz.SelectedIndex], "game.json");

                //reload page
                Frame.Navigate(typeof(Games));
            }
        }
    }
}
