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
using CAA_CrossPlatform.UWP.Models;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.Drawing;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageGamePlayer : Page
    {
        //setup event
        Event selectedEvent = new Event();

        //setup game stuff
        Game game = new Game();
        int selectedQuestion = 0;
        List<Question> questions = new List<Question>();
        List<Answer> answers = new List<Answer>();

        //setup correct and incorrect
        List<bool> correctAnswers = new List<bool>();
        List<bool> userAnswers = new List<bool>();

        public PageGamePlayer()
        {
            this.InitializeComponent();
            this.Loaded += PageGamePlayer_Loaded;
        }

        //create question object
        StackPanel CreateQuestion(Question Question, List<Answer> Answers, bool isEnabled)
        {
            //create stackpanel
            StackPanel spQuestion = new StackPanel();
            spQuestion.Margin = new Thickness(0, 10, 0, 0);
            spQuestion.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(175, 255, 255, 255));
            spQuestion.Padding = new Thickness(5);

            //setup question
            TextBlock tbQuestion = new TextBlock();
            tbQuestion.Text = Question.name;
            tbQuestion.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 0, 0));
            spQuestion.Children.Add(tbQuestion);

            //setup answers
            foreach (Answer answer in Answers)
            {
                CheckBox chkAnswer = new CheckBox();
                chkAnswer.Content = answer.name;
                chkAnswer.IsEnabled = isEnabled;
                spQuestion.Children.Add(chkAnswer);
            }

            //return stackpanel
            return spQuestion;
        }

        //get results
        StackPanel GetResults()
        {
            //create stackpanel
            StackPanel spResults = new StackPanel();
            spResults.Margin = new Thickness(0, 10, 0, 0);
            spResults.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(175, 255, 255, 255));
            spResults.Padding = new Thickness(5);

            //setup answers
            foreach (Question question in questions)
            {
                TextBlock tbQuestion = new TextBlock();
                tbQuestion.Text = question.name;
                tbQuestion.FontSize = 22;
                tbQuestion.Margin = new Thickness(0, 10, 0, 0);
                spResults.Children.Add(tbQuestion);
                List<Answer> tempAnswers = new List<Answer>();
                foreach (Answer answer in answers)
                    if (answer.QuestionID == question.Id)
                    {
                        int index = answers.IndexOf(answer);
                        TextBlock tbAnswer = new TextBlock();
                        tbAnswer.FontSize = 20;
                        tbAnswer.Text = "- " + answer.name;
                        if (answer.correct)
                            tbAnswer.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 84, 191, 90));
                        else if (!answer.correct && userAnswers[index])
                            tbAnswer.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 214, 49, 49));
                        spResults.Children.Add(tbAnswer);
                    }
            }

            //return stackpanel
            return spResults;
        }

        private async void PageGamePlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //get game and questions
            selectedEvent = EnvironmentModel.Event;
            game = EnvironmentModel.Game;
            questions = EnvironmentModel.QuestionList;
            answers = EnvironmentModel.AnswerList;
            EnvironmentModel.Reset();

            lblGame.Text = game.name;

            //load answers for first question
            Question question = questions[0];
            List<Answer> tempAnswers = new List<Answer>();
            foreach (Answer answer in answers)
                if (answer.QuestionID == question.Id)
                {
                    tempAnswers.Add(answer);
                    correctAnswers.Add(answer.correct);
                }

            //add answer
            spQuestions.Children.Add(CreateQuestion(question, tempAnswers, true));

            //get image
            if (!string.IsNullOrEmpty(game.imagePath))
            {
                try
                {
                    StorageFolder images = await ApplicationData.Current.LocalFolder.GetFolderAsync("images");
                    StorageFile imageFile = await images.GetFileAsync(game.imagePath);
                    BitmapImage image = new BitmapImage(new Uri(imageFile.Path));
                    background.ImageSource = image;
                }
                catch { }
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //view results
            if (selectedQuestion == questions.Count - 1)
            {
                //hide submit button
                btnSubmit.Visibility = Visibility.Collapsed;

                //submit answers
                foreach (StackPanel spQuestion in spQuestions.Children)
                {
                    foreach (dynamic element in spQuestion.Children)
                    {
                        if (element.GetType() == typeof(CheckBox))
                        {
                            CheckBox chkCorrect = (CheckBox)element;
                            userAnswers.Add(chkCorrect.IsChecked.GetValueOrDefault());
                        }
                    }
                }

                //reset questions
                spQuestions.Children.Clear();

                //get results
                spQuestions.Children.Add(GetResults());
            }

            //submit question then go to next
            else if (selectedQuestion != questions.Count - 1)
            {
                //submit answers
                foreach (StackPanel spQuestion in spQuestions.Children)
                {
                    foreach (dynamic element in spQuestion.Children)
                    {
                        if (element.GetType() == typeof(CheckBox))
                        {
                            CheckBox chkCorrect = (CheckBox)element;
                            userAnswers.Add(chkCorrect.IsChecked.GetValueOrDefault());
                        }
                    }
                }

                //reset questions
                spQuestions.Children.Clear();

                //next question
                selectedQuestion++;

                //get question and answers
                Question question = questions[selectedQuestion];
                List<Answer> tempAnswers = new List<Answer>();
                foreach (Answer answer in answers)
                    if (answer.QuestionID == question.Id)
                        tempAnswers.Add(answer);

                //add answer
                spQuestions.Children.Add(CreateQuestion(question, tempAnswers, true));
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentModel.Event = selectedEvent;
            Frame.GoBack();
        }
    }
}
