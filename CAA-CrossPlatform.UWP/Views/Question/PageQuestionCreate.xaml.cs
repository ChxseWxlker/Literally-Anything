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
    public sealed partial class PageQuestionCreate : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        public PageQuestionCreate()
        {
            this.InitializeComponent();
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

        private async void CreateQuestion_Click(object sender, RoutedEventArgs e)
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
            {
                //validate title
                if (q.name.ToLower().Trim() == QuestionTxt.Text.ToLower().Trim() && q.hidden == false)
                {
                    QuestionTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That question already exists, please enter a different name").ShowAsync();
                    return;
                }
                if (q.name.ToLower().Trim() == QuestionTxt.Text.ToLower().Trim() && q.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That question is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        q.hidden = false;
                        Json.Edit(q, "question.json");
                        Frame.Navigate(typeof(PageQuestion));
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //create question object
            Question question = new Question();

            //set object properties
            question.name = QuestionTxt.Text;
            /*
            question.answers = new List<string>();
            question.correctAnswers = new List<bool>();

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    if (Answer1Txt.Text != "")
                    {
                        question.answers.Add(Answer1Txt.Text);
                        question.correctAnswers.Add(Answer1CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 1)
                {
                    if (Answer2Txt.Text != "")
                    {
                        question.answers.Add(Answer2Txt.Text);
                        question.correctAnswers.Add(Answer2CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 2)
                {
                    if (Answer3Txt.Text != "")
                    {
                        question.answers.Add(Answer3Txt.Text);
                        question.correctAnswers.Add(Answer3CorrectChk.IsChecked ?? false);
                    }
                }

                else if (i == 3)
                {
                    if (Answer4Txt.Text != "")
                    {
                        question.answers.Add(Answer4Txt.Text);
                        question.correctAnswers.Add(Answer4CorrectChk.IsChecked ?? false);
                    }
                }
            }

            //write json to file
            Json.Write(question, "question.json");
            */
            //navigate back to question page
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
