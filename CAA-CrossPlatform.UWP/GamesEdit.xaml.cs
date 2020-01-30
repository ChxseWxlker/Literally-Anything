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

        private async void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = Json.Read("game.json");

            //validation
            if (QuizTxt.Text == "")
            {
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
            {
                //validate title
                if (g.title.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == false)
                {
                    await new MessageDialog("That quiz already exists, please enter different name").ShowAsync();
                    return;
                }
                else if (g.title.ToLower().Trim() == QuizTxt.Text.ToLower().Trim() && g.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That quiz is hidden, would you like to reactivate it?");
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
