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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class GamesEdit : Page
    {
        public GamesEdit()
        {
            this.InitializeComponent();
        }

        Game selectedGame;
        List<Question> listQuestions = new List<Question>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get current game
            selectedGame = (Game)e.Parameter;

            //get list of questions
            List<Question> questions = Json.Read("question.json");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    listQuestions.Add(q);
                    if (selectedGame.questions.Contains(q.id))
                        lstQuestions.SelectedItems.Add(q.name);
                }

            QuizTxt.Text = selectedGame.title;
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

        private void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            //create list of selected questions
            selectedGame.questions = new List<int>();

            foreach(string sq in lstQuestions.SelectedItems)
                foreach (Question q in listQuestions)
                    if (sq == q.name)
                        selectedGame.questions.Add(q.id);

            selectedGame.title = QuizTxt.Text;

            //edit game object
            Json.Edit(selectedGame, "game.json");

            //redirect to game page
            Frame.Navigate(typeof(Games));
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void QuizTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
