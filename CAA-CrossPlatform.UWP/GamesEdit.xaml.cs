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
    public sealed partial class GamesEdit : Page
    {
        public GamesEdit()
        {
            this.InitializeComponent();
        }
        Game selectedGame;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //gets the selected game
            List<Question> questions = Json.Read("question.json");
            foreach (Question q in questions)
            {
                lstQuestions.Items.Add(q.name);
            }
            List<Game> games = Json.Read("game.json");
           
            
            QuizTxt.Text = games[Convert.ToInt32(e.Parameter)].title;
            
            //Checks the questions already selected
            foreach (Object o in lstQuestions.Items)
            {
                foreach(int q in games[Convert.ToInt32(e.Parameter)].questions)
                {
                    if(lstQuestions.Items.IndexOf(o) == q)
                    {
                        lstQuestions.SelectedItems.Add(o);
                    }
                }
            }
            selectedGame = games[Convert.ToInt32(e.Parameter)];





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

        private void EditQuiz_Click(object sender, RoutedEventArgs e)
        {
            List<Game> games = Json.Read("game.json");
            foreach (Game g in games)
            {
                if (g.title == selectedGame.title)
                {
                    g.title = QuizTxt.Text;
                    g.questions = new List<int>();
                }
            }
            
            foreach(string s in lstQuestions.SelectedItems)
            {
                foreach(string st in lstQuestions.Items)
                {
                    if(s == st)
                    {
                         foreach(Game g in games)
                        {
                            if (g.title == selectedGame.title)
                            {
                                g.questions.Add(lstQuestions.Items.IndexOf(s));
                            }
                        }
                    }

                }
                
            }
            Frame.Navigate(typeof(Games));
        }
    }
}
