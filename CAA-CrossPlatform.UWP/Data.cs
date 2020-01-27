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
    public class Event
    {
        public Event()
        {
            this.hidden = false;
        }

        //event properties
        public int id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string location { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int game { get; set; }
        public bool memberOnly { get; set; }
        public bool trackGuestNum { get; set; }
        public bool trackAdultNum { get; set; }
        public bool trackChildNum { get; set; }
    }

    public class RootEvent
    {
        public List<Event> events { get; set; }
    }

    public class Game
    {
        public Game()
        {
            this.hidden = false;
        }

        //game properties
        public int id { get; set; }
        public bool hidden { get; set; }
        public string title { get; set; }
        public List<int> questions { get; set; }
    }

    public class RootGame
    {
        public List<Game> games { get; set; }
    }

    public class Question
    {
        public Question()
        {
            this.hidden = false;
        }

        //question properties
        public int id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public List<string> answers { get; set; }
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

                //event json object
                if (model.ToString().Substring(6, 4) == "even")
                {
                    jsonObj = new List<Event>();
                    foreach (dynamic gEvent in model.events)
                    {
                        Event e = new Event();
                        e.id = gEvent.id;
                        e.hidden = gEvent.hidden;
                        e.name = gEvent.name;
                        e.location = gEvent.location;
                        e.startDate = gEvent.startDate;
                        e.endDate = gEvent.endDate;
                        e.game = gEvent.game;
                        e.trackGuestNum = gEvent.trackGuestNum;
                        e.trackAdultNum = gEvent.trackAdultNum;
                        e.trackChildNum = gEvent.trackChildNum;
                        jsonObj.Add(e);
                    }
                }

                //game json object
                else if (model.ToString().Substring(6, 4) == "game")
                {
                    jsonObj = new List<Game>();
                    foreach (dynamic game in model.games)
                    {
                        Game g = new Game();
                        g.id = game.id;
                        g.hidden = game.hidden;
                        g.title = game.title;
                        g.questions = new List<int>();
                        foreach (int question in game.questions)
                            g.questions.Add(question);
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
                        q.hidden = question.hidden;
                        q.name = question.name;
                        q.answers = new List<string>();
                        foreach (string answer in question.answers)
                            q.answers.Add(answer);
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

            //event json object
            if (model.GetType().ToString().Substring(56, 4) == "Even")
            {
                //initialize root
                root = new RootEvent();

                //set object properties
                root.events = model;
                int id = root.events[root.events.Count - 1].id + 1;
                jsonObj.id = id;
                root.events.Add(jsonObj);
            }

            //game json object
            else if (model.GetType().ToString().Substring(56, 4) == "Game")
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

        public static void Edit(dynamic jsonObj, string fileName)
        {
            //get file path
            string path = ApplicationData.Current.LocalFolder.Path + @"\" + fileName;

            //get existing model
            dynamic model = Read(fileName);

            //setup root
            dynamic root = null;

            //event json object
            if (model.GetType().ToString().Substring(56, 4) == "Even")
            {
                //initialize root
                root = new RootEvent();

                //set object properties
                root.events = model;
                int editIndex = -1;
                foreach (Event ev in root.events)
                    if (ev.id == jsonObj.id)
                        editIndex = root.events.IndexOf(ev);
                if (editIndex != -1)
                    root.events.RemoveAt(editIndex);
                root.events.Insert(editIndex, jsonObj);
            }

            //game json object
            else if (model.GetType().ToString().Substring(56, 4) == "Game")
            {
                //initialize root
                root = new RootGame();

                //set object properties
                root.games = model;
                int editIndex = -1;
                foreach (Game g in root.games)
                    if (g.id == jsonObj.id)
                        editIndex = root.games.IndexOf(g);
                if (editIndex != -1)
                    root.games.RemoveAt(editIndex);
                root.games.Insert(editIndex, jsonObj);
            }

            //question json object
            else if (model.GetType().ToString().Substring(56, 4) == "Ques")
            {
                //initialize root
                root = new RootQuestion();

                //set object properties
                root.questions = model;
                int editIndex = -1;
                foreach (Question q in root.questions)
                    if (q.id == jsonObj.id)
                        editIndex = root.questions.IndexOf(q);
                if (editIndex != -1)
                    root.questions.RemoveAt(editIndex);
                root.questions.Insert(editIndex, jsonObj);
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
