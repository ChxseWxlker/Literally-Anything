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

        public async void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {








            //check json read
            //string checkList = "";

            //List<Game> games = Json.Read("game.json");
            //foreach (Game g in games)
            //{
            //    checkList += $"ID:{g.id}\n";
            //}

            //checkList += "\n";
            
            //List<Question> questions = Json.Read("question.json");
            //foreach (Question q in questions)
            //{
            //    checkList += $"ID {q.id} | Question {q.name} | Answer {q.correct}\n";
            //}
            //await new MessageDialog(checkList).ShowAsync();

            ////check json write
            //Game game = new Game();
            //game.questions = new int[] { 1, 2, 3 };
            //Json.Write(game, "game.json");

            //Question question = new Question();
            //question.name = "Is json sick?";
            //question.answers = new string[] { "Yes", "No" };
            //question.correct = "Yes";
            //Json.Write(question, "question.json");
        }
        public async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            string checkList = "";
            List<Game> games = Json.Read("game.json");
            foreach (Game g in games)
            {
                checkList += $"ID:{g.id}\n";
            }

            checkList += "\n";

            List<Question> questions = Json.Read("question.json");
            foreach (Question q in questions)
            {
                checkList += $"ID {q.id} | Question {q.name} | Answer {q.correct}\n";
            }
           // await new MessageDialog(checkList).ShowAsync();


            foreach (Question q in questions)
            {
                if(q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                }
                
            }

        }
        private void QuizTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
    }
}
