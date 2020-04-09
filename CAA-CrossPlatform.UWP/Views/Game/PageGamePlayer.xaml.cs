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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageGamePlayer : Page
    {
        //setup event
        Event selectedEvent = new Event();

        //setup game stuff
        Game game = new Game();
        List<Question> questions = new List<Question>();
        List<Answer> answers = new List<Answer>();

        public PageGamePlayer()
        {
            this.InitializeComponent();
            this.Loaded += PageGamePlayer_Loaded;
        }

        private void PageGamePlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //get game and questions
            selectedEvent = EnvironmentModel.Event;
            game = EnvironmentModel.Game;
            questions = EnvironmentModel.QuestionList;
            answers = EnvironmentModel.AnswerList;
            EnvironmentModel.Reset();

            lblGame.Text = game.name;
            lblQuestions.Text = questions.Count.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            EnvironmentModel.Event = selectedEvent;
            Frame.GoBack();
        }
    }
}
