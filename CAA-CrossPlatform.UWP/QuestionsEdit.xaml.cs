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
    public sealed partial class QuestionsEdit : Page
    {
        public QuestionsEdit()
        {
            this.InitializeComponent();
        }

        Question testQuestion;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<Question> questions = Json.Read("question.json");

            int checkID = Convert.ToInt32(e.Parameter);

            foreach (Question q in questions)
            {
                if(q.id == checkID)
                {
                    testQuestion = q;
                }
            }

            QuestionTxt.Text = testQuestion.name;

            for(int i = 0; i < testQuestion.answers.Count; i++)
            {
                switch(i)
                {
                    case 0:
                        Answer1Txt.Text = testQuestion.answers[i];
                        break;
                    case 1:
                        Answer2Txt.Text = testQuestion.answers[i];
                        break;
                    case 2:
                        Answer3Txt.Text = testQuestion.answers[i];
                        break;
                    case 3:
                        Answer2Txt.Text = testQuestion.answers[i];
                        break;



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

        private void QuestionTB_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void EditQuestion_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
