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
    public sealed partial class PageGameEdit : Page
    {
        public PageGameEdit()
        {
            this.InitializeComponent();
        }

        Game selectedGame;
        List<Question> listQuestions = new List<Question>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get current game
            selectedGame = (Game)e.Parameter;
            /*
            //get list of questions
            List<Question> questions = Json.Read("question.json");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    listQuestions.Add(q);
                    if (selectedGame.questions.Contains(q.Id))
                        lstQuestions.SelectedItems.Add(q.name);
                }
                */
            QuizTxt.Text = selectedGame.name;
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private async void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = Json.Read("game.json");

            //validation
            if (QuizTxt.Text == "")
            {
                QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
                //validate title
                if (g.name.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == true)
                {
                    QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That quiz already exists, please enter a different name").ShowAsync();
                    return;
                }
            /*
            //create list of selected questions
            selectedGame.questions = new List<int>();

            foreach(string sq in lstQuestions.SelectedItems)
                foreach (Question q in listQuestions)
                    if (sq == q.name)
                        selectedGame.questions.Add(q.id);

            selectedGame.title = QuizTxt.Text;
            */
            //edit game object
            Json.Edit(selectedGame, "game.json");

            //redirect to game page
            Frame.Navigate(typeof(PageGame));
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }
    }
}
