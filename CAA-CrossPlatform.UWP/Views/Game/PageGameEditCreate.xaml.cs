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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageGameEditCreate : Page
    {
        //list of questions
        static List<Question> visibleQuestions = new List<Question>();

        //setup selected game
        Game selectedGame = null;

        //setup selected questions
        List<Question> selectedQuestions = null;

        public PageGameEditCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageGameEditCreate_Loaded;
        }

        private async void PageGameEditCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //set selected game
            selectedGame = EnvironmentModel.Game;

            //set selected questions
            selectedQuestions = EnvironmentModel.QuestionList;
            EnvironmentModel.QuestionList = null;

            //get name
            if (selectedGame != null)
                txtGame.Text = selectedGame.name;

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
            if (selectedQuestions != null)
            {
                foreach (Question question in selectedQuestions)
                    lbQuestion.SelectedItems.Add(question.name);
            }

            //database selections
            else if (selectedQuestions == null && selectedGame != null)
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
                if (game.name.ToLower().Trim() == txtGame.Text.ToLower().Trim() && game.hidden == false && selectedGame == null)
                {
                    await new MessageDialog("That game already exists, enter a different name.").ShowAsync();
                    return;
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

            if (selectedGame != null)
            {
                newGame.Id = selectedGame.Id;
                await Connection.Update(newGame);
            }

            else if (selectedGame == null)
                newGame.Id = await Connection.Insert(newGame);

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
            //clear questions
            lbQuestion.Items.Clear();
            
            //get all questions
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                foreach (Question question in visibleQuestions)
                    lbQuestion.Items.Add(question.name);
            }

            //search questions
            else
                foreach (Question question in visibleQuestions)
                    if (question.name.ToLower().Trim().Contains(txtSearch.Text.ToLower().Trim()))
                        lbQuestion.Items.Add(question.name);
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
                EnvironmentModel.QuestionList = null;

            Game game = new Game();
            game.name = txtGame.Text;
            EnvironmentModel.Game = game;
            Frame.Navigate(typeof(PageQuestionEditCreate));
        }
    }
}
