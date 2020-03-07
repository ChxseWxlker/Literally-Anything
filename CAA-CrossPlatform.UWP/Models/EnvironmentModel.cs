using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class EnvironmentModel
    {
        public static Answer Answer { get; set; }
        public static Attendance Attendance { get; set; }
        public static AttendanceItem AttendanceItem { get; set; }
        public static Event Event { get; set; }
        public static EventItem EventItem { get; set; }
        public static Game Game { get; set; }
        public static GameQuestion GameQuestion { get; set; }
        public static Item Item { get; set; }
        public static Question Question { get; set; }

        public static List<Answer> AnswerList { get; set; }
        public static List<Attendance> AttendanceList { get; set; }
        public static List<AttendanceItem> AttendanceItemList { get; set; }
        public static List<Event> EventList { get; set; }
        public static List<EventItem> EventItemList { get; set; }
        public static List<Game> GameList { get; set; }
        public static List<GameQuestion> GameQuestionList { get; set; }
        public static List<Item> ItemList { get; set; }
        public static List<Question> QuestionList { get; set; }

        public static void Reset()
        {
            Answer = null;
            Attendance = null;
            AttendanceItem = null;
            Event = null;
            EventItem = null;
            Game = null;
            GameQuestion = null;
            Item = null;
            Question = null;

            AnswerList = null;
            AttendanceList = null;
            AttendanceItemList = null;
            EventList = null;
            EventItemList = null;
            GameList = null;
            GameQuestionList = null;
            ItemList = null;
            QuestionList = null;
        }
    }
}
