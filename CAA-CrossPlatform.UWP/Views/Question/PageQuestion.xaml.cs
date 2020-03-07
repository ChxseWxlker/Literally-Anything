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
        //setup api
        static ApiHandler api = new ApiHandler();

        //get list of questions
        List<Question> listQuestions = new List<Question>();

        public PageQuestion()
        {
            this.InitializeComponent();
            this.Loaded += PageQuestion_Loaded;
        }

        private async void PageQuestion_Loaded(object sender, RoutedEventArgs e)
        {
            //get all questions
            List<Question> questions = await Connection.Get("Question");

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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestionEditCreate));
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedIndex == -1)
                await new MessageDialog("Please choose a question to edit").ShowAsync();
            else
                Frame.Navigate(typeof(PageQuestionEdit), listQuestions[lstQuestions.SelectedIndex]);
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedIndex == -1)
                await new MessageDialog("Please choose a question to delete").ShowAsync();
            else
            {
                //delete question
                await Connection.Delete(listQuestions[lstQuestions.SelectedIndex]);

                //reload page
                Frame.Navigate(typeof(PageQuestion));
            }
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
    }
}
