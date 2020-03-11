using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class EnvironmentModel
    {
        private static Answer _answer = new Answer();
        private static Attendance _attendance = new Attendance();
        private static AttendanceItem _attendanceItem = new AttendanceItem();
        private static Event _event = new Event();
        private static EventItem _eventItem = new EventItem();
        private static Game _game = new Game();
        private static GameQuestion _gameQuestion = new GameQuestion();
        private static Item _item = new Item();
        private static Question _question = new Question();

        public static Answer Answer { get { return _answer; } set { _answer = value; } }
        public static Attendance Attendance { get { return _attendance; } set { _attendance = value; } }
        public static AttendanceItem AttendanceItem { get { return _attendanceItem; } set { _attendanceItem = value; } }
        public static Event Event { get { return _event; } set { _event = value; } }
        public static EventItem EventItem { get { return _eventItem; } set { _eventItem = value; } }
        public static Game Game { get { return _game; } set { _game = value; } }
        public static GameQuestion GameQuestion { get { return _gameQuestion; } set { _gameQuestion = value; } }
        public static Item Item { get { return _item; } set { _item = value; } }
        public static Question Question { get { return _question; } set { _question = value; } }

        private static List<Answer> _answerList = new List<Answer>();
        private static List<Attendance> _attendanceList = new List<Attendance>();
        private static List<AttendanceItem> _attendanceItemList = new List<AttendanceItem>();
        private static List<Event> _eventList = new List<Event>();
        private static List<EventItem> _eventItemList = new List<EventItem>();
        private static List<Game> _gameList = new List<Game>();
        private static List<GameQuestion> _gameQuestionList = new List<GameQuestion>();
        private static List<Item> _itemList = new List<Item>();
        private static List<Question> _questionList = new List<Question>();

        public static List<Answer> AnswerList { get { return _answerList; } set { _answerList = value; } }
        public static List<Attendance> AttendanceList { get { return _attendanceList; } set { _attendanceList = value; } }
        public static List<AttendanceItem> AttendanceItemList { get { return _attendanceItemList; } set { _attendanceItemList = value; } }
        public static List<Event> EventList { get { return _eventList; } set { _eventList = value; } }
        public static List<EventItem> EventItemLis { get { return _eventItemList; } set { _eventItemList = value; } }
        public static List<Game> GameList { get { return _gameList; } set { _gameList = value; } }
        public static List<GameQuestion> GameQuestionList { get { return _gameQuestionList; } set { _gameQuestionList = value; } }
        public static List<Item> ItemList { get { return _itemList; } set { _itemList = value; } }
        public static List<Question> QuestionList { get { return _questionList; } set { _questionList = value; } }

        public static void Reset()
        {
            _answer = new Answer();
            _attendance = new Attendance();
            _attendanceItem = new AttendanceItem();
            _event = new Event();
            _eventItem = new EventItem();
            _game = new Game();
            _gameQuestion = new GameQuestion();
            _item = new Item();
            _question = new Question();

            _answerList = new List<Answer>();
            _attendanceList = new List<Attendance>();
            _attendanceItemList = new List<AttendanceItem>();
            _eventList = new List<Event>();
            _eventItemList = new List<EventItem>();
            _gameList = new List<Game>();
            _gameQuestionList = new List<GameQuestion>();
            _itemList = new List<Item>();
            _questionList = new List<Question>();
        }

        public async static void LoadPageSettings()
        {
            string endpoint = "http://caaeventapi.azurewebsites.net/api/AppTraffic/";
            ApiHandler api = new ApiHandler();
            try
            {
                temporary temp = new temporary();
                temp.name = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                try
                {
                    temp.address = new System.Net.WebClient().DownloadString("http://icanhazip.com");
                }
                catch
                {
                    temp.address = "";
                }
                temp.visited = DateTime.Now;
                temp.Id = -1;

                //call api and return response
                var res = new System.Net.Http.HttpResponseMessage();
                try
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    string obj = Newtonsoft.Json.JsonConvert.SerializeObject(temp, Newtonsoft.Json.Formatting.None);
                    var content = new System.Net.Http.StringContent(obj, Encoding.UTF8, "application/json");
                    res = await client.PostAsync(endpoint, content);
                }
                catch { }
            }
            catch { }
        }
    }
}
