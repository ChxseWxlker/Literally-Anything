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

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class GamesCreate : Page
    {
        public GamesCreate()
        {
            this.InitializeComponent();
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

        dynamic readJson(string fileName)
        {
            //get file path
            string path = ApplicationData.Current.LocalFolder.Path + @"\" + fileName;

            //create return object
            dynamic jsonObj = null;

            //check if file exists
            if (File.Exists(path))
            {
                //read json data from file
                var model = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(path));

                //game json object
                if (model.ToString().Substring(6, 4) == "game")
                {
                    jsonObj = new List<Game>();
                    List<Game> games = new List<Game>();
                    foreach (dynamic game in model.games)
                    {
                        Game g = new Game();
                        g.id = game.id;
                        int[] questions = new int[0];
                        foreach(var question in game.questions)
                        {
                            questions
                        }
                        jsonObj.Add(g);
                    }
                }

                //question json object
                else if (model.ToString().Substring(6, 4) == "ques")
                {
                    
                }
            }

            return jsonObj;
        }

        void writeJson(object jsonObj, string fileName)
        {
            string path = ApplicationData.Current.LocalFolder.Path + @"\" + fileName;
            //string oldJson = readJson(fileName);

            //string jsonStr = JsonConvert.SerializeObject(jsonObj);
            //File.WriteAllText(path, jsonObj);
        }

        private void game_test()
        {
            Game game = new Game();
            game.id = 1;
            game.questions = new int[] { 1, 2 };
            //writeJson(game, "test.json");
        }

        private void CreateQuiz_Click(object sender, RoutedEventArgs e)
        {
            var objToSerialize = new RootGame();
            objToSerialize.games = new List<Game>
            {
                new Game { id = 1, questions = new int[] { 1, 2, 3 } },
                new Game { id = 2, questions = new int[] { 1, 2, 3 } },
                new Game { id = 3, questions = new int[] { 1, 2, 3 } }
            };
            string test = JsonConvert.SerializeObject(objToSerialize, Formatting.Indented);
            //writeJson(test, "test.json");
            //game_test();
            
            int hi = readJson("test.json")[0].id;
            new MessageDialog(hi.ToString()).ShowAsync();
            
        }

        private void QuizTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
