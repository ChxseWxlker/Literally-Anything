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
using CAA_CrossPlatform.UWP.Models;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class GamesCreate : Page
    {
        //list of questions
        static List<Question> visibleQuestions = new List<Question>();

        public GamesCreate()
        {
            this.InitializeComponent();
            this.Loaded += GamesCreate_Loaded;
        }

        private async void GamesCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get question list
            List<Question> questions = await Connection.Get("Question");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    visibleQuestions.Add(q);
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
            List<Game> games = await Connection.Get("Game");

            //validation
            if (QuizTxt.Text == "")
            {
                QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
            {
                //validate title
                if (g.title.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == false)
                {
                    QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That quiz already exists, please enter a different name").ShowAsync();
                    return;
                }

                //unhide game if user chooses
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
                        Connection.Update(g);
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
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            lstQuestions.Items.Clear();
            foreach (Question q in visibleQuestions)
                if (q.name.ToLower().Trim().Contains(TxtSearch.Text.ToLower().Trim()))
                    lstQuestions.Items.Add(q.name);
        }

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(TxtSearch.Text == "Search")
            TxtSearch.Text = "";
        }
    }
}
