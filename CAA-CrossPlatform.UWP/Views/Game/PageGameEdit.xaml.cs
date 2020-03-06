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
        //setup api
        static ApiHandler api = new ApiHandler();

        public PageGameEdit()
        {
            this.InitializeComponent();
        }

        Game selectedGame;
        List<Question> visibleQuestions = new List<Question>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get current game
            selectedGame = (Game)e.Parameter;

            //get list of game questions
            List<GameQuestion> gameQuestions = await Connection.Get("GameQuestion");

            //get list of questions
            List<Question> questions = await Connection.Get("Question");
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lbQuestion.Items.Add(q.name);
                    visibleQuestions.Add(q);

                    foreach (GameQuestion gq in gameQuestions)
                        if (gq.GameID == selectedGame.Id && gq.QuestionID == q.Id)
                            lbQuestion.SelectedItems.Add(q.name);
                }

            //setup name
            txtQuiz.Text = selectedGame.name;
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
            List<Game> games = await Connection.Get("Game");

            //validation
            if (txtQuiz.Text == "")
            {
                QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please enter a quiz name").ShowAsync();
                return;
            }

            foreach (Game g in games)
                //validate title
                if (g.name.ToLower().Trim() == txtQuiz.Text.ToLower().Trim() && g.hidden == true)
                {
                    QuizNameTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That quiz already exists, please enter a different name").ShowAsync();
                    return;
                }

            //get list of questions
            List<Question> questions = await Connection.Get("Question");

            //reset game questions
            List<GameQuestion> gameQuestions = await Connection.Get("GameQuestion");
            foreach (GameQuestion gq in gameQuestions)
                if (gq.GameID == selectedGame.Id)
                    await Connection.Delete(gq);

            //create game question links
            foreach (Question q in questions)
                if (lbQuestion.SelectedItems.Contains(q.name))
                {
                    GameQuestion gq = new GameQuestion();
                    gq.GameID = selectedGame.Id;
                    gq.QuestionID = q.Id;
                    await Connection.Insert(gq);
                }

            selectedGame.name = txtQuiz.Text;
            
            //edit game object
            Connection.Update(selectedGame);

            //redirect
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void CancelQuiz_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }

        private void btnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //get menu button
            Button btn = (Button)sender;

            //event
            if (btn.Content.ToString().Contains("Event"))
                Frame.Navigate(typeof(PageEvent));

            //game
            else if (btn.Content.ToString().Contains("Game"))
                Frame.Navigate(typeof(PageGame));

            //question
            else if (btn.Content.ToString().Contains("Question"))
                Frame.Navigate(typeof(PageQuestion));
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //prompt user
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout?",
                Content = "You will be redirected to the home page and locked out until you log back in. Are you sure you want to logout?",
                PrimaryButtonText = "Logout",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult logoutRes = await logoutDialog.ShowAsync();
        }
    }
}
