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
        List<Question> visibleQuestions = new List<Question>();

        public PageQuestion()
        {
            this.InitializeComponent();
            this.Loaded += PageQuestion_Loaded;
        }

        private async void PageQuestion_Loaded(object sender, RoutedEventArgs e)
        {
            //reset environment vars
            EnvironmentModel.Reset();

            //get all questions
            List<Question> questions = await Connection.Get("Question");

            //add question if visible
            foreach (Question q in questions)
                if (q.hidden == false)
                {
                    lbQuestion.Items.Add(q.name);
                    visibleQuestions.Add(q);
                }
        }

        private async void btnControls_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            if (btnSender.Name.Contains("Create"))
                Frame.Navigate(typeof(PageQuestionEditCreate));

            else if (btnSender.Name.Contains("Edit"))
            {
                if (lbQuestion.SelectedIndex == -1)
                {
                    await new MessageDialog("Choose a question to edit.").ShowAsync();
                    return;
                }

                EnvironmentModel.Question = visibleQuestions[lbQuestion.SelectedIndex];
                Frame.Navigate(typeof(PageQuestionEditCreate));
            }

            else if (btnSender.Name.Contains("Delete"))
            {
                if (lbQuestion.SelectedIndex == -1)
                {
                    await new MessageDialog("Choose a question to delete.").ShowAsync();
                    return;
                }

                await Connection.Delete(visibleQuestions[lbQuestion.SelectedIndex]);

                Frame.Navigate(typeof(PageQuestion));
            }
        }
    }
}
