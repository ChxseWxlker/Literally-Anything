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

        private async void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = Json.Read("game.json");

            //validation
            if (QuizTxt.Text == "")
            {
                QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                QuizTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
            {
                //validate title
                if (g.title.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == false)
                {
                    QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    QuizTxt.Style = (Style)Application.Current.Resources["TxtValidationFailedTemplate"];
                    await new MessageDialog("That quiz already exists, please enter a different name").ShowAsync();
                    return;
                }
                else if (g.title.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That quiz is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        g.hidden = false;
                        Json.Edit(g, "game.json");
                        Frame.Navigate(typeof(Games));
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //create game object
            Game game = new Game();

            //set object properties
            game.questions = new List<int>();

            foreach (string sq in lstQuestions.SelectedItems)
                foreach (Question q in listQuestions)
                    if (sq == q.name)
                        game.questions.Add(q.id);

            game.title = QuizTxt.Text;

            //save to json
            Json.Write(game, "game.json");

            //navigate back to game
            Frame.Navigate(typeof(Games));
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            lstQuestions.Items.Clear();
            foreach (Question q in listQuestions)
            {
                if (q.name.ToLower().Trim().Contains(TxtSearch.Text.ToLower().Trim()))
                {
                    lstQuestions.Items.Add(q.name);
                }
            }
        }

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Search")
                TxtSearch.Text = "";
        }

        
    }
}
