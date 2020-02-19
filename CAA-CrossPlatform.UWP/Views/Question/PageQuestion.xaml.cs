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
    public sealed partial class PageQuestion : Page
    {
        //get list of questions
        List<Question> listQuestions = new List<Question>();

        public PageQuestion()
        {
            this.InitializeComponent();
            this.Loaded += PageQuestion_Loaded;
        }

        private void PageQuestion_Loaded(object sender, RoutedEventArgs e)
        {
            //get all questions
            List<Question> questions = Json.Read("question.json");

            //add question if visible
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lstQuestions.Items.Add(q.name);
                    listQuestions.Add(q);
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

        private void CreateQuestion_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestionCreate));
        }

        private async void EditQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedIndex == -1)
                await new MessageDialog("Please choose a question to edit").ShowAsync();
            else
                Frame.Navigate(typeof(PageQuestionEdit), listQuestions[lstQuestions.SelectedIndex]);
        }

        private async void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedIndex == -1)
                await new MessageDialog("Please choose a question to delete").ShowAsync();
            else
            {
                //hide question object
                listQuestions[lstQuestions.SelectedIndex].hidden = true;

                //edit question object
                Json.Edit(listQuestions[lstQuestions.SelectedIndex], "question.json");

                //reload page
                Frame.Navigate(typeof(PageQuestion));
            }
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }
    }
}
