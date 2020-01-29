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
        //list of questions
        List<Question> listQuestions = new List<Question>();

        public GamesCreate()
        {
            this.InitializeComponent();
            this.Loaded += GamesCreate_Loaded;
        }

        private void GamesCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get question list
            List<Question> questions = Json.Read("question.json");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    listQuestions.Add(q);
                }
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

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            //create game object
            Game game = new Game();

            //set object properties
            game.questions = new List<int>();

            foreach (string sq in lstQuestions.SelectedItems)
                foreach (Question q in listQuestions)
                    if (sq == q.name)
                        game.questions.Add(q.id);

            game.title = QuizTxt.Text;
            Json.Write(game, "game.json");

            //navigate back to game
            Frame.Navigate(typeof(Games));
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }
    }
}
