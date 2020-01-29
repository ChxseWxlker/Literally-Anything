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
    public sealed partial class QuestionsEdit : Page
    {
        //create question object
        Question selectedQuestion = new Question();

        public QuestionsEdit()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //set question object
            selectedQuestion = (Question)e.Parameter;

            //set text
            QuizTxt.Text = selectedQuestion.name;

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

        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            //set object properties
            selectedQuestion.name = QuizTxt.Text;

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

            //redirect to questions page
            Frame.Navigate(typeof(Questions));
        }

        private void QuizNameTB_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CancelQuestion_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }
    }
}
