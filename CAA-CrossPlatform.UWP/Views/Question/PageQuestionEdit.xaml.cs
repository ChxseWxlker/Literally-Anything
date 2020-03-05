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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //set question object
            selectedQuestion = (Question)e.Parameter;

            //set text
            txtQuestion.Text = selectedQuestion.name;

            //get answers
            List<Answer> answers = new List<Answer>();
            foreach (Answer a in await Connection.Get("Answer"))
                if (a.QuestionID == selectedQuestion.Id)
                    answers.Add(a);

            //create objects
            txtAnswer.Text = answers[0].name;
            chkCorrect.IsChecked = answers[0].correct;
            answers.RemoveAt(0);
            foreach (Answer a in answers)
            {
                //create stack panel
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new Thickness(0, 40, 0, 0);

                //create textbox
                TextBox txt = new TextBox();
                txt.Text = a.name;
                txt.TextWrapping = TextWrapping.Wrap;
                txt.FontSize = 25;
                txt.Width = 400;

                //create checkbox
                CheckBox chk = new CheckBox();
                chk.IsChecked = a.correct;
                chk.Margin = new Thickness(40, 0, 0, 0);
                chk.Width = 25;

                //append items
                sp.Children.Add(txt);
                sp.Children.Add(chk);
                spAnswersPanel.Children.Add(sp);
            }
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

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            //get list of questions
            List<Question> questions = await Connection.Get("Question");

            //validation
            if (txtQuestion.Text == "")
            {
                QuestionTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                await new MessageDialog("Please enter a question name").ShowAsync();
                return;
            }

            foreach (Question q in questions)
            {
                //validate title
                if (q.name.ToLower().Trim() == txtQuestion.Text.ToLower().Trim() && q.hidden == false)
                {
                    QuestionTB.Style = (Style)Application.Current.Resources["ValidationFailedTemplate"];
                    await new MessageDialog("That question already exists, please enter a different name").ShowAsync();
                    return;
                }
                if (q.name.ToLower().Trim() == txtQuestion.Text.ToLower().Trim() && q.hidden == true)
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
                        Connection.Update(q);
                        Frame.Navigate(typeof(PageQuestion));
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //create question
            selectedQuestion.name = txtQuestion.Text;
            Connection.Update(selectedQuestion);

            //create list of answers
            foreach (StackPanel sp in spAnswersPanel.Children)
            {
                TextBox txt = (TextBox)sp.Children[0];
                CheckBox chk = (CheckBox)sp.Children[1];

                //create answer
                if (txt.Text != "")
                {
                    Answer answer = new Answer();
                    answer.name = txt.Text;
                    answer.correct = chk.IsChecked ?? false;
                    answer.QuestionID = selectedQuestion.Id;
                    answer.Id = await Connection.Insert(answer);
                }
            }

            //navigate
            Frame.Navigate(Frame.BackStack.Last().SourcePageType);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
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

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //create stack panel
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Margin = new Thickness(0, 40, 0, 0);

            //create textbox
            TextBox txt = new TextBox();
            txt.TextWrapping = TextWrapping.Wrap;
            txt.FontSize = 25;
            txt.Width = 400;

            //create checkbox
            CheckBox chk = new CheckBox();
            chk.Margin = new Thickness(40, 0, 0, 0);
            chk.Width = 25;

            //append items
            sp.Children.Add(txt);
            sp.Children.Add(chk);
            spAnswersPanel.Children.Add(sp);
        }
    }
}