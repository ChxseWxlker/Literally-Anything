using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CAA_CrossPlatform.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Games : Page
    {
        public Games()
        {
            this.InitializeComponent();
        }



       private void Page_OnLoad(object sender, RoutedEventArgs e)
        {
            

            
            
        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamesCreate));
        }

        private void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GamesEdit), lstQuiz.SelectedIndex);
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
            List<Game> games = Json.Read("game.json");
            
            foreach(Game g in games)
            {
                lstQuiz.Items.Add(g.title);
            }
            




        }

        private void DeleteQuiz_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DelteQuiz_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
