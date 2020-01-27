using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CAA_CrossPlatform.UWP
{
    public class Game
    {
        public int id { get; set; }
        public int[] questions { get; set; }
    }

    public class RootGame
    {
        public List<Game> games { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public string name { get; set; }
        public string[] answers { get; set; }
        public string correct { get; set; }
    }

    public class RootQuestion
    {
        public List<Question> questions { get; set; }
    }

    public class Json
    {
        public static dynamic Read(string fileName)
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
                    foreach (dynamic game in model.games)
                    {
                        Game g = new Game();
                        g.id = game.id;
                        g.questions = new int[game.questions.Count];
                        for (int i = 0; i < game.questions.Count; i++)
                        {
                            g.questions[i] = game.questions[i];
                        }
                        jsonObj.Add(g);
                    }
                }

                //question json object
                else if (model.ToString().Substring(6, 4) == "ques")
                {
                    jsonObj = new List<Question>();
                    foreach (dynamic question in model.questions)
                    {
                        Question q = new Question();
                        q.id = question.id;
                        q.name = question.name;
                        q.answers = new string[question.answers.Count];
                        for (int i = 0; i < question.answers.Count; i++)
                        {
                            q.answers[i] = question.answers[i];
                        }
                        q.correct = question.correct;
                        jsonObj.Add(q);
                    }
                }

                //json object doesn't exist
                else
                {
                    return null;
                }
            }

            //return json object list
            return jsonObj;
        }

        public static void Write(dynamic jsonObj, string fileName)
        {
            //get file path
            string path = ApplicationData.Current.LocalFolder.Path + @"\" + fileName;

            //get existing model
            dynamic model = Read(fileName);

            //setup root
            dynamic root = null;

            //game json object
            if (model.GetType().ToString().Substring(56, 4) == "Game")
            {
                //initialize root
                root = new RootGame();

                //set object properties
                root.games = model;
                int id = root.games[root.games.Count - 1].id + 1;
                jsonObj.id = id;
                root.games.Add(jsonObj);
            }

            //question json object
            else if (model.GetType().ToString().Substring(56, 4) == "Ques")
            {
                //initialize root
                root = new RootQuestion();

                //set object properties
                root.questions = model;
                int id = root.questions[root.questions.Count - 1].id + 1;
                jsonObj.id = id;
                root.questions.Add(jsonObj);
            }

            //json object doesn't exist
            else
            {
                return;
            }

            //serialize and write to file
            string jsonStr = JsonConvert.SerializeObject(root, Formatting.Indented);
            File.WriteAllText(path, jsonStr);
        }
    }
}
