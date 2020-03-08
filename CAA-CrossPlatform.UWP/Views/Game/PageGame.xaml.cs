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
    public sealed partial class PageGame : Page
    {
        //create list of visible games
        List<Game> visibleGames = new List<Game>();

        public PageGame()
        {
            this.InitializeComponent();
            this.Loaded += PageGame_Loaded;
        }

        private async void PageGame_Loaded(object sender, RoutedEventArgs e)
        {
            //reset environment vars
            EnvironmentModel.Reset();

            //get list of games
            List<Game> games = await Connection.Get("Game");

            foreach (Game game in games)
                if (game.hidden == false)
                {
                    lbGame.Items.Add(game.name);
                    visibleGames.Add(game);
                }
        }

        private async void btnControl_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            if (btnSender.Name.Contains("Create"))
                Frame.Navigate(typeof(PageGameEditCreate));

            else if (btnSender.Name.Contains("Edit"))
            {
                if (lbGame.SelectedIndex == -1)
                {
                    await new MessageDialog("Choose a game to edit.").ShowAsync();
                    return;
                }

                //set environment var
                EnvironmentModel.Game = visibleGames[lbGame.SelectedIndex];

                //edit game
                Frame.Navigate(typeof(PageGameEditCreate));
            }

            else if (btnSender.Name.Contains("Delete"))
            {
                if (lbGame.SelectedIndex == -1)
                {
                    await new MessageDialog("Choose a game to delete.").ShowAsync();
                    return;
                }

                //remove game object
                await Connection.Delete(visibleGames[lbGame.SelectedIndex]);

                //reload page
                Frame.Navigate(typeof(PageGame));
            }
        }
    }
}
