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
    public sealed partial class PageQuestionEditCreate : Page
    {
        //create list of answers
        List<Answer> answers = new List<Answer>();

        //setup selected question
        Question selectedQuestion = new Question();

        public PageQuestionEditCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageQuestionEditCreate_Loaded;
        }

        private async void PageQuestionEditCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get selected question
            selectedQuestion = EnvironmentModel.Question;
            EnvironmentModel.Question = new Question();

            //set properties
            if (selectedQuestion.Id != 0)
            {
                //setup button
                if (selectedQuestion.Id != -1)
                    btnSubmit.Content = "Save";

                //set properties
                txtQuestion.Text = selectedQuestion.name;
                btnCreate.Content = "Save";

                //get answers
                List<Answer> tempAnswers = await Connection.Get("Answer");

                foreach (Answer answer in tempAnswers)
                    if (answer.QuestionID == selectedQuestion.Id)
                    {
                        //add to list
                        answers.Add(answer);

                        //populate native textbox first
                        if (string.IsNullOrEmpty(txtAnswer.Text))
                        {
                            txtAnswer.Text = answer.name;
                            chkCorrect.IsChecked = answer.correct;
                            txtAnswer.Name = $"txtAnswer_{answer.Id}";
                        }

                        //create more textboxes
                        else if (!string.IsNullOrEmpty(txtAnswer.Text))
                        {
                            StackPanel spAnswerNew = new StackPanel();
                            spAnswerNew.Margin = new Thickness(0, 10, 0, 0);
                            spAnswerNew.Orientation = Orientation.Horizontal;

                            TextBox txtAnswerNew = new TextBox();
                            txtAnswerNew.Name = $"txtAnswer_{answer.Id}";
                            txtAnswerNew.Text = answer.name;
                            txtAnswerNew.HorizontalAlignment = HorizontalAlignment.Left;
                            txtAnswerNew.TextWrapping = TextWrapping.Wrap;
                            txtAnswerNew.FontSize = 25;
                            txtAnswerNew.Width = 300;
                            txtAnswerNew.TextChanged += txtAnswer_TextChanged;

                            CheckBox chkCorrectNew = new CheckBox();
                            chkCorrectNew.Name = $"chkCorrect_{answer.Id}";
                            chkCorrectNew.IsChecked = answer.correct;
                            chkCorrectNew.Margin = new Thickness(40, 0, 0, 0);
                            chkCorrectNew.Width = 25;
                            chkCorrectNew.MinWidth = 0;

                            //append to stackpanel
                            spAnswerNew.Children.Add(txtAnswerNew);
                            spAnswerNew.Children.Add(chkCorrectNew);
                            spAnswersPanel.Children.Add(spAnswerNew);
                        }
                    }

                //create empty textbox under
                if (answers.Count > 0)
                {
                    StackPanel spAnswerNew = new StackPanel();
                    spAnswerNew.Margin = new Thickness(0, 10, 0, 0);
                    spAnswerNew.Orientation = Orientation.Horizontal;

                    TextBox txtAnswerNew = new TextBox();
                    txtAnswerNew.Name = "txtAnswer";
                    txtAnswerNew.HorizontalAlignment = HorizontalAlignment.Left;
                    txtAnswerNew.TextWrapping = TextWrapping.Wrap;
                    txtAnswerNew.FontSize = 25;
                    txtAnswerNew.Width = 300;
                    txtAnswerNew.TextChanged += txtAnswer_TextChanged;

                    CheckBox chkCorrectNew = new CheckBox();
                    chkCorrectNew.Name = "chkCorrect";
                    chkCorrectNew.Margin = new Thickness(40, 0, 0, 0);
                    chkCorrectNew.Width = 25;
                    chkCorrectNew.MinWidth = 0;

                    //append to stackpanel
                    spAnswerNew.Children.Add(txtAnswerNew);
                    spAnswerNew.Children.Add(chkCorrectNew);
                    spAnswersPanel.Children.Add(spAnswerNew);
                }
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //check name
            if (string.IsNullOrEmpty(txtQuestion.Text))
            {
                await new MessageDialog("Question name is required.").ShowAsync();
                return;
            }

            //check answers and correct
            int emptyAnswerCount = 0;
            int emptyCorrectCount = 0;
            foreach (StackPanel spAnswer in spAnswersPanel.Children)
            {
                TextBox txtAnswer = (TextBox)spAnswer.Children[0];
                CheckBox chkCorrect = (CheckBox)spAnswer.Children[1];

                string answer = txtAnswer.Text;
                bool correct = chkCorrect.IsChecked ?? false;

                if (string.IsNullOrEmpty(answer))
                    emptyAnswerCount++;

                if (!correct)
                    emptyCorrectCount++;
            }

            //no answers
            if (emptyAnswerCount == spAnswersPanel.Children.Count)
            {
                await new MessageDialog("Minimum of 1 answer is required.").ShowAsync();
                return;
            }

            //no correct answers
            if (emptyCorrectCount == spAnswersPanel.Children.Count)
            {
                await new MessageDialog("Minimum of 1 correct answer is required.").ShowAsync();
                return;
            }

            //get list of questions
            List<Question> questions = await Connection.Get("Question");

            foreach (Question question in questions)
            {
                //validate title
                if (question.name.ToLower().Trim() == txtQuestion.Text.ToLower().Trim() && question.hidden == false)
                {
                    if (selectedQuestion.Id == 0 || selectedQuestion.Id == -1)
                    {
                        await new MessageDialog("That question already exists, enter a different name.").ShowAsync();
                        return;
                    }
                }
                if (question.name.ToLower().Trim() == txtQuestion.Text.ToLower().Trim() && question.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That question is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        question.hidden = false;
                        await Connection.Update(question);
                        Frame.Navigate(typeof(PageQuestion));
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //create question object
            Question newQuestion = new Question();

            //create question
            newQuestion.name = txtQuestion.Text;

            if (selectedQuestion.Id == 0 || selectedQuestion.Id == -1)
                newQuestion.Id = await Connection.Insert(newQuestion);

            else
            {
                newQuestion.Id = selectedQuestion.Id;
                await Connection.Update(newQuestion);
            }

            //create answers
            if (newQuestion.Id != -1)
            {
                //remove old answers
                foreach (Answer answer in answers)
                    await Connection.Delete(answer);

                foreach (StackPanel spAnswer in spAnswersPanel.Children)
                {
                    TextBox txtAnswer = (TextBox)spAnswer.Children[0];
                    CheckBox chkCorrect = (CheckBox)spAnswer.Children[1];

                    if (!string.IsNullOrEmpty(txtAnswer.Text))
                    {
                        Answer answer = new Answer();
                        answer.name = txtAnswer.Text;
                        answer.correct = chkCorrect.IsChecked ?? false;
                        answer.QuestionID = newQuestion.Id;
                        answer.Id = await Connection.Insert(answer);
                    }
                }
            }

            //navigate
            Frame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void txtAnswer_TextChanged(object sender, TextChangedEventArgs e)
        {
            //get textbox
            TextBox txtSender = (TextBox)sender;
            StackPanel spSender = (StackPanel)txtSender.Parent;

            //create next answer
            if (spAnswersPanel.Children.IndexOf(spSender) == spAnswersPanel.Children.Count - 1)
            {
                if (!string.IsNullOrEmpty(txtSender.Text))
                {
                    StackPanel spAnswerNew = new StackPanel();
                    spAnswerNew.Margin = new Thickness(0, 10, 0, 0);
                    spAnswerNew.Orientation = Orientation.Horizontal;

                    TextBox txtAnswerNew = new TextBox();
                    txtAnswerNew.Name = "txtAnswer";
                    txtAnswerNew.HorizontalAlignment = HorizontalAlignment.Left;
                    txtAnswerNew.TextWrapping = TextWrapping.Wrap;
                    txtAnswerNew.FontSize = 25;
                    txtAnswerNew.Width = 300;
                    txtAnswerNew.TextChanged += txtAnswer_TextChanged;

                    CheckBox chkCorrectNew = new CheckBox();
                    chkCorrectNew.Name = "chkCorrect";
                    chkCorrectNew.Margin = new Thickness(40, 0, 0, 0);
                    chkCorrectNew.Width = 25;
                    chkCorrectNew.MinWidth = 0;

                    //append to stackpanel
                    spAnswerNew.Children.Add(txtAnswerNew);
                    spAnswerNew.Children.Add(chkCorrectNew);
                    spAnswersPanel.Children.Add(spAnswerNew);
                }
            }

            //remove answer
            else if (string.IsNullOrEmpty(txtSender.Text) && spAnswersPanel.Children.Count > 1)
            {
                //focus other track answer
                try
                {
                    StackPanel spPrevious = (StackPanel)spAnswersPanel.Children[spAnswersPanel.Children.IndexOf(spSender) - 1];
                    TextBox txtFocus = (TextBox)spPrevious.Children[0];
                    txtFocus.Focus(FocusState.Keyboard);
                }
                catch
                {
                    StackPanel spFirst = (StackPanel)spAnswersPanel.Children[0];
                    TextBox txtFocus = (TextBox)spFirst.Children[0];
                    txtFocus.Focus(FocusState.Keyboard);
                }

                //remove child
                spAnswersPanel.Children.Remove(spSender);
            }
        }
    }
}
