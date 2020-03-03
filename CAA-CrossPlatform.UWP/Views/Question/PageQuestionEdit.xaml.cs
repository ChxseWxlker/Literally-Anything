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
    public sealed partial class PageQuestionEdit : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //create question object
        Question selectedQuestion = new Question();

        public PageQuestionEdit()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //set question object
            selectedQuestion = (Question)e.Parameter;

            //set text
            QuestionTxt.Text = selectedQuestion.name;
            /*
            for (int i = 0; i < selectedQuestion.answers.Count; i++)
            {
                if (i == 0)
                {
                    Answer1Txt.Text = selectedQuestion.answers[i];
                    Answer1CorrectChk.IsChecked = selectedQuestion.correctAnswers[i];
                }

                else if (i == 1)
                {
                    Answer2Txt.Text = selectedQuestion.answers[i];
                    Answer2CorrectChk.IsChecked = selectedQuestion.correctAnswers[i];
                }

                else if (i == 2)
                {
                    Answer3Txt.Text = selectedQuestion.answers[i];
                    Answer3CorrectChk.IsChecked = selectedQuestion.correctAnswers[i];
                }

                else if (i == 3)
                {
                    Answer4Txt.Text = selectedQuestion.answers[i];
                    Answer4CorrectChk.IsChecked = selectedQuestion.correctAnswers[i];
                }
            }
            */
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

        private async void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            //get list of questions
            List<Question> questions = Json.Read("question.json");

            //validation
            if (QuestionTxt.Text == "")
            {
                QuestionTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please enter a question name").ShowAsync();
                return;
            }

            foreach (Question q in questions)
                //validate name
                if (q.name.ToLower().Trim() == QuestionTxt.Text.ToLower().Trim() && q.hidden == true)
                {
                    QuestionTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That question already exists, please enter a different name").ShowAsync();
                    return;
                }

            //set object properties
            selectedQuestion.name = QuestionTxt.Text;
            /*
            selectedQuestion.answers = new List<string>();
            selectedQuestion.correctAnswers = new List<bool>();

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    if (Answer1Txt.Text != "")
                    {
                        selectedQuestion.answers.Add(Answer1Txt.Text);
                        selectedQuestion.correctAnswers.Add(Answer1CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 1)
                {
                    if (Answer2Txt.Text != "")
                    {
                        selectedQuestion.answers.Add(Answer2Txt.Text);
                        selectedQuestion.correctAnswers.Add(Answer2CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 2)
                {
                    if (Answer3Txt.Text != "")
                    {
                        selectedQuestion.answers.Add(Answer3Txt.Text);
                        selectedQuestion.correctAnswers.Add(Answer3CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 3)
                {
                    if (Answer4Txt.Text != "")
                    {
                        selectedQuestion.answers.Add(Answer4Txt.Text);
                        selectedQuestion.correctAnswers.Add(Answer4CorrectChk.IsChecked ?? false);
                    }
                }
            }

            //save json object
            Json.Edit(selectedQuestion, "question.json");
            */
            //redirect to questions page
            Frame.Navigate(typeof(PageQuestion));
        }

        private void CancelQuestion_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
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