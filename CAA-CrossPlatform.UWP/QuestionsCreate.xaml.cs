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
    public sealed partial class QuestionsCreate : Page
    {
        public QuestionsCreate()
        {
            this.InitializeComponent();
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

        private async void CreateQuestion_Click(object sender, RoutedEventArgs e)
        {
            //get list of questions
            List<Question> questions = Json.Read("question.json");

            //validation
            if (QuestionTxt.Text == "")
            {
                await new MessageDialog("Please enter a question name").ShowAsync();
                return;
            }

            foreach (Question q in questions)
            {
                //validate title
                if (q.name.ToLower().Trim() == QuestionTxt.Text.ToLower().Trim() && q.hidden == false)
                {
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
                        Frame.Navigate(typeof(Questions));
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

            //navigate back to question page
            Frame.Navigate(typeof(Questions));
        }

        private void CancelQuestion_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventExcel));
        }
    }
}
