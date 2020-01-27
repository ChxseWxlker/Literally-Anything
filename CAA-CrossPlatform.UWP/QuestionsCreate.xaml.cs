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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CAA_CrossPlatform.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

        private void QuizNameTB_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            List<string> answers = new List<string>();
            answers.Add(Answer1Txt.Text);
            answers.Add(Answer2Txt.Text);
            answers.Add(Answer3Txt.Text);
            answers.Add(Answer4Txt.Text);


            Question question = new Question();
            question.name = QuestionTxt.Text;
            question.answers = new List<string>();
            foreach (string a in answers)
            {
                if (a.Length > 0)
                {
                    question.answers.Add(a);
                    
                }
            }
            if(Answer1Chk.IsChecked == true && Answer1Txt.Text.Length > 0)
            {
                question.correct = Answer1Txt.Text;
            }
            else if (Answer2Chk.IsChecked == true && Answer2Txt.Text.Length > 0)
            {
                question.correct = Answer2Txt.Text;
            }
            else if (Answer3Chk.IsChecked == true && Answer3Txt.Text.Length > 0)
            {
                question.correct = Answer3Txt.Text;
            }
            else if (Answer4Chk.IsChecked == true && Answer4Txt.Text.Length > 0)
            {
                question.correct = Answer4Txt.Text;
            }

            Json.Write(question, "question.json");
        }
    }
}
