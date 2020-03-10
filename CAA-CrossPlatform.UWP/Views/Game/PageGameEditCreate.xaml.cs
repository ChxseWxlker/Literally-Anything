using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CAA_CrossPlatform.UWP.Models;
using Windows.UI;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageGameEditCreate : Page
    {
        //list of questions
        static List<Question> visibleQuestions = new List<Question>();

        //setup selected game
        Game selectedGame = new Game();

        //setup selected questions
        List<Question> selectedQuestions = new List<Question>();

        public PageGameEditCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageGameEditCreate_Loaded;
        }

        private async void PageGameEditCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //set selected game
            selectedGame = EnvironmentModel.Game;
            EnvironmentModel.Game = new Game();

            //set selected questions
            selectedQuestions = EnvironmentModel.QuestionList;
            EnvironmentModel.QuestionList = new List<Question>();

            if (selectedGame.Id != 0)
            {
                //setup button
                if (selectedGame.Id != -1)
                    btnSubmit.Content = "Save";

                //set properties
                txtGame.Text = selectedGame.name;
            }

            //get question list
            List<Question> questions = await Connection.Get("Question");
            foreach (Question question in questions)
                if (question.hidden == false)
                {
                    //add to list
                    lbQuestion.Items.Add(question.name);
                    visibleQuestions.Add(question);
                }

            List<GameQuestion> gameQuestions = await Connection.Get("GameQuestion");

            //callback selections
            if (selectedQuestions.Count != 0)
            {
                foreach (Question question in selectedQuestions)
                    lbQuestion.SelectedItems.Add(question.name);
            }

            //database selections
            else if (selectedQuestions.Count == 0 && selectedGame.Id != 0)
            {
                foreach (Question question in visibleQuestions)
                    foreach (GameQuestion gameQuestion in gameQuestions)
                        if (gameQuestion.QuestionID == question.Id && gameQuestion.GameID == selectedGame.Id)
                            lbQuestion.SelectedItems.Add(question.name);
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //get list of games
            List<Game> games = await Connection.Get("Game");

            //validation
            if (string.IsNullOrEmpty(txtGame.Text))
            {
                await new MessageDialog("Enter a game name.").ShowAsync();
                txtGame.Focus(FocusState.Keyboard);
                return;
            }

            foreach (Game game in games)
            {
                //validate name
                if (game.name.ToLower().Trim() == txtGame.Text.ToLower().Trim() && game.hidden == false)
                {
                    if (selectedGame.Id == 0 || selectedGame.Id == -1)
                    {
                        await new MessageDialog("That game already exists, enter a different name.").ShowAsync();
                        return;
                    }
                }

                //unhide game if user chooses
                else if (game.name.ToLower().Trim() == txtGame.Text.ToLower().Trim() && game.hidden == true)
                {
                    MessageDialog msg = new MessageDialog("That game is hidden, would you like to re-activate it?");
                    msg.Commands.Add(new UICommand("Yes") { Id = 1 });
                    msg.Commands.Add(new UICommand("No") { Id = 0 });
                    msg.CancelCommandIndex = 0;
                    var choice = await msg.ShowAsync();

                    //re-activate game
                    if ((int)choice.Id == 1)
                    {
                        game.hidden = false;
                        await Connection.Update(game);
                        Frame.GoBack();
                        return;
                    }

                    else if ((int)choice.Id == 0)
                        return;
                }
            }

            //setup game object
            Game newGame = new Game();
            newGame.name = txtGame.Text;

            if (selectedGame.Id == 0 || selectedGame.Id == -1)
                newGame.Id = await Connection.Insert(newGame);

            else
            {
                newGame.Id = selectedGame.Id;
                await Connection.Update(newGame);
            }

            //get all game questions
            List<GameQuestion> gameQuestions = await Connection.Get("GameQuestion");

            //create questions
            foreach (Question question in visibleQuestions)
                if (lbQuestion.SelectedItems.Contains(question.name))
                {
                    //remove old game questions
                    foreach (GameQuestion gameQuestionOld in gameQuestions)
                        if (gameQuestionOld.GameID == newGame.Id)
                            await Connection.Delete(gameQuestionOld);

                    //create game question
                    GameQuestion gameQuestion = new GameQuestion();
                    gameQuestion.GameID = newGame.Id;
                    gameQuestion.QuestionID = question.Id;

                    //save to database
                    gameQuestion.Id = await Connection.Insert(gameQuestion);
                }

            //navigate back
            Frame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower().Replace(" ", "");
            lbQuestion.Items.Clear();
            SolidColorBrush btnBg = (SolidColorBrush)btnSearch.Background;

            //change to clear
            if (btnBg.Color.G.ToString() == "82")
            {
                foreach (Question question in visibleQuestions)
                    if (question.name.ToLower().Replace(" ", "").Contains(search))
                        lbQuestion.Items.Add(question.name);

                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplateRed"];
                btnSearch.Content = "\uE894";
            }

            //change to search
            else if (btnBg.Color.G.ToString() == "14")
            {
                foreach (Question question in visibleQuestions)
                    lbQuestion.Items.Add(question.name);

                txtSearch.Text = "";
                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplate"];
                btnSearch.Content = "\uE1A3";
            }
        }

        private void btnCreateQuestion_Click(object sender, RoutedEventArgs e)
        {
            List<Question> questions = new List<Question>();

            foreach (string questionName in lbQuestion.SelectedItems)
                foreach (Question question in visibleQuestions)
                    if (question.name == questionName)
                        questions.Add(question);

            EnvironmentModel.QuestionList = questions;

            if (questions.Count == 0)
                EnvironmentModel.QuestionList = new List<Question>();

            Game game = new Game();
            game.Id = -1;
            game.name = txtGame.Text;
            EnvironmentModel.Game = game;
            Frame.Navigate(typeof(PageQuestionEditCreate));
        }
    }
}
