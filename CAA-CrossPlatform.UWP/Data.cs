using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Popups;

namespace CAA_CrossPlatform.UWP
{
    public class Event
    {
        public Event()
        {
            this.hidden = false;
            this.memberOnly = true;
            this.trackGuestNum = false;
            this.trackAdultNum = false;
            this.trackChildNum = false;
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
        public List<bool> correctAnswers { get; set; }
    }

    public class RootQuestion
    {
        public List<Question> questions { get; set; }
    }

    public class Json
    {
        public static void Verify(string fileName)
        {
            string path = ApplicationData.Current.LocalFolder.Path + @"\" + fileName;
            dynamic root = null;

            if (!File.Exists(path))
            {
                if (fileName == "event.json")
                {
                    List<Event> events = new List<Event>();
                    root = new RootEvent();
                    root.events = events;
                }

                else if (fileName == "game.json")
                {
                    List<Game> games = new List<Game>();
                    root = new RootGame();
                    root.games = games;
                }

                else if (fileName == "question.json")
                {
                    List<Question> questions = new List<Question>();
                    root = new RootQuestion();
                    root.questions = questions;
                }

                string jsonStr = JsonConvert.SerializeObject(root, Formatting.Indented);
                File.WriteAllText(path, jsonStr);
            }
        }

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
                        q.correctAnswers = new List<bool>();
                        foreach (bool correct in question.correctAnswers)
                            q.correctAnswers.Add(correct);
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
                int id = 1;
                if (root.events.Count != 0)
                    id = root.events[root.events.Count - 1].id + 1;
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
                int id = 1;
                if (root.games.Count != 0)
                    id = root.games[root.games.Count - 1].id + 1;
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
                int id = 1;
                if (root.questions.Count != 0)
                    id = root.questions[root.questions.Count - 1].id + 1;
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

    public class Excel
    {
        public static async Task<List<Event>> Load()
        {
            //create file picker
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".csv");

            //choose file
            var file = await picker.PickSingleFileAsync();

            //create rows
            IList<string> excelDoc = await FileIO.ReadLinesAsync(file);
            List<string> rows = new List<string>();

            //create object
            List<Event> events = new List<Event>();
            foreach (string row in excelDoc)
                if (row.Length > 0 && excelDoc.IndexOf(row) > 0)
                {
                    //create cells
                    string[] cells = row.Split(',');

                    //add cell values to object
                    Event ev = new Event();
                    List<Event> jsonEvents = Json.Read("event.json");
                    foreach (Event jEvent in jsonEvents)
                        if (jEvent.name.ToLower().Trim() == cells[0].ToLower().Trim())
                            ev.id = Convert.ToInt32(jEvent.id);
                    ev.name = cells[0];
                    ev.location = cells[1];
                    ev.startDate = Convert.ToDateTime(cells[2]);
                    ev.endDate = Convert.ToDateTime(cells[3]);
                    List<Game> games = Json.Read("game.json");
                    foreach (Game game in games)
                        if (game.title.ToLower().Trim() == cells[4].ToLower().Trim())
                            ev.game = Convert.ToInt32(game.id);
                    ev.memberOnly = Convert.ToBoolean(cells[5]);
                    events.Add(ev);
                }

            return events;
        }

        public static async void Write()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            try
            {
                StorageFile sampleFile = await folder.CreateFileAsync("sample.txt",
                CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, "Swift as a shadow");
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        public static async Task<dynamic> Read()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".csv");

            var file = await picker.PickSingleFileAsync();
            string excelDoc = await FileIO.ReadTextAsync(file);

            string[] rows = excelDoc.Split('\n');

            List<string> items = new List<string>();
            for (int i = 1; i < rows.Length; i++)
            {
                for (int x = 0; x < rows[i].Split(',').Length; x++)
                {
                    string[] cells = rows[i].Split(',');
                    if (cells[x] != "")
                        items.Add(cells[x]);
                }
            }

            string hi = "";
            foreach (string item in items)
            {
                hi += item + "\n";
            }
            /*
            string[] textTest = text.Split('\n');
            string hi = "";
            foreach (string h in textTest)
                if (h != "")
                    hi += $"|{h.Replace("\n", "")}|\n";
            */
            await new MessageDialog(hi).ShowAsync();
            return hi;
        }
    }
}
    