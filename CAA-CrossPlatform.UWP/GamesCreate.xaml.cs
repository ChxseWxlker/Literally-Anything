using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class GamesCreate : Page
    {
        public GamesCreate()
        {
            this.InitializeComponent();
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

        private async void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
           
            //check json write
            List<int> questionList = new List<int>();
            foreach(string s in lstQuestions.SelectedItems)
            {
                foreach(string st in lstQuestions.Items)
                {
                    if(s == st)
                    {
                        questionList.Add(lstQuestions.Items.IndexOf(s));
                    }
                }
                
            }
            
            //Creates a mew game with the provided data
            Game game = new Game();
            game.title = QuizTxt.Text;
            game.questions = questionList;
            Json.Write(game, "game.json");

            Frame.Navigate(typeof(Games));
        }

        private void QuizTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //check json read
           

            List<Game> games = Json.Read("game.json");
            
            List<Question> questions = Json.Read("question.json");
            foreach(Question q in questions)
            {
                lstQuestions.Items.Add(q.name);
            }
            
            

            
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }
    }
}
