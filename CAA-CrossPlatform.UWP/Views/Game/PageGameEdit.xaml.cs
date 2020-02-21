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

        private void btnShowPane_Click(object sender, RoutedEventArgs e)
        {
            svMenu.IsPaneOpen = !svMenu.IsPaneOpen;
            if (svMenu.IsPaneOpen)
            {
                btnShowPane.Content = "\uE00E";
                btnEventMenu.Visibility = Visibility.Visible;
                btnGameMenu.Visibility = Visibility.Visible;
                btnQuestionMenu.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPane.Content = "\uE00F";
                btnEventMenu.Visibility = Visibility.Collapsed;
                btnGameMenu.Visibility = Visibility.Collapsed;
                btnQuestionMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void svMenu_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            //hide buttons
            btnShowPane.Content = "\uE00F";
            btnEventMenu.Visibility = Visibility.Collapsed;
            btnGameMenu.Visibility = Visibility.Collapsed;
            btnQuestionMenu.Visibility = Visibility.Collapsed;
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

            //log user out
            if (logoutRes == ContentDialogResult.Primary)
            {
                //reset active username
                Environment.SetEnvironmentVariable("activeUser", "");

                //update menu
                txtAccount.Text = "";

                //logout
                api.Logout();

                //redirect to index
                Frame.Navigate(typeof(PageIndex));
            }
        }
    }
}
