using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using CAA_CrossPlatform.UWP.Models;

namespace CAA_CrossPlatform.UWP
{
    //class to connect to sqlite database and fetch data
    public class Connection
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //get path for database
        private static string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "EventsDB.db");
        private static SqliteConnection con = new SqliteConnection($"Filename={path}");

        //checks if the database exists
        private async static void Verify()
        {
            //database doesn't exist
            if (!File.Exists(path))
            {
                //create database file
                await ApplicationData.Current.LocalFolder.CreateFileAsync("EventsDB.db");

                //try connecting to the database
                try
                {
                    //open connection
                    con.Open();

                    //create game table
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE 'Game' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL UNIQUE, " +
                    "'EventID' INTEGER NOT NULL, FOREIGN KEY('EventID') REFERENCES 'Event'('Id') );" +

                    //create event table
                    "CREATE TABLE 'Event' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, " +
                    "'name'  TEXT NOT NULL, 'displayName' TEXT NOT NULL, 'nameAbbrev' TEXT NOT NULL UNIQUE, 'startDate' TEXT NOT NULL, 'endDate' TEXT NOT NULL, 'memberOnly' " +
                    "INTEGER NOT NULL DEFAULT 1, 'GameID' INTEGER NOT NULL, FOREIGN KEY('GameID') REFERENCES 'Game'('Id')); " +

                    //create question table
                    "CREATE TABLE 'Question' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL, 'GameID' " +
                    "INTEGER NOT NULL, FOREIGN KEY('GameID') REFERENCES 'Game'('Id') );" +

                    //create answer table
                    "CREATE TABLE 'Answer' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL, 'correct' " +
                    "INTEGER NOT NULL, 'QuestionID' INTEGER NOT NULL, FOREIGN KEY('QuestionID') REFERENCES 'Question'('Id') );" +

                    //create game question table
                    "CREATE TABLE 'GameQuestion' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'GameID' INTEGER NOT NULL, 'QuestionID' INTEGER NOT NULL, " +
                    "FOREIGN KEY('GameID') REFERENCES 'Game'('Id'), FOREIGN KEY('QuestionID') REFERENCES 'Question'('Id') );" +

                    //create tracking info table
                    "CREATE TABLE 'TrackingInfo' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'item' TEXT NOT NULL, " +
                    "'amount' INTEGER NOT NULL DEFAULT 0, 'EventID' INTEGER NOT NULL, FOREIGN KEY('EventID') REFERENCES 'Event'('Id') );" +

                    //create item table
                    "CREATE TABLE 'Item' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'itemName' TEXT NOT NULL, 'valueType' TEXT);" +

                    //create eventitem table
                    "CREATE TABLE 'EventItem' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMEMT UNIQUE, 'EventId' INTEGER NOT NULL, 'ItemId' INTEGER NOT NULL, " +
                    "FOREIGN KEY('EventId') REFERENCES 'Event'('Id'), FOREIGN KEY('ItemId') REFERENCES 'Item'('Id'));" +

                    //create attendanceitem table
                    "CREATE TABLE 'AttendanceItem' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'AttendanceId' INTEGER NOT NULL, 'EventItemId' INTEGER NOT NULL, " +
                    "'Answer' TEXT NOT NULL, FOREIGN KEY('AttendanceId') REFERENCES 'Attendance'('Id'), FOREIGN KEY('EventItemId') REFERENCES 'EventItem'('Id'));" +

                    //create attendance table
                    "CREATE TABLE 'Attendance' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'memberNumber' TEXT, 'arriveTime' TEXT NOT NULL, 'isMember' INTEGER, " +
                    "'phone' TEXT, 'firstName' TEXT, 'lastName' TEXT, 'EventID' INTEGER NOT NULL, FOREIGN KEY('EventID') REFERENCES 'Event'('Id') );", con);

                    //create tables
                    cmd.ExecuteNonQuery();

                    //close connection
                    if (con.State == System.Data.ConnectionState.Open)
                        con.Close();
                }

                //catch errors
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
        }

        //returns a specific record or all records
        public static async Task<dynamic> Get(string Table, int? Id = null)
        {
            //use api if available
            if (api.CheckConnection())
                return await api.GET(Table, Id);

            //verify the database exists
            Verify();

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //select all records
                SqliteCommand cmd = new SqliteCommand($"SELECT * FROM {Table};", con);

                //select single record
                if (Id != null)
                    cmd = new SqliteCommand($"SELECT * FROM {Table} WHERE Id = {Id};", con);

                //query record(s)
                SqliteDataReader query = cmd.ExecuteReader();

                //all records
                if (Id == null)
                {
                    //get correct list of records
                    dynamic records = null;
                    if (Table == "Event")
                        records = new List<Event>();
                    else if (Table == "Game")
                        records = new List<Game>();
                    else if (Table == "Question")
                        records = new List<Question>();
                    else if (Table == "Answer")
                        records = new List<Answer>();
                    else if (Table == "GameQuestion")
                        records = new List<GameQuestion>();
                    else if (Table == "Item")
                        records = new List<Item>();
                    else if (Table == "EventItem")
                        records = new List<EventItem>();
                    else if (Table == "AttendanceItem")
                        records = new List<AttendanceItem>();
                    else if (Table == "Attendance")
                        records = new List<Attendance>();

                    while (query.Read())
                    {
                        //event table
                        if (Table == "Event")
                        {
                            Event e = new Event();
                            e.Id = Convert.ToInt32(query[0]);
                            e.hidden = Convert.ToBoolean(query[1]);
                            e.name = query[2].ToString();
                            e.displayName = query[3].ToString();
                            e.nameAbbrev = query[4].ToString();
                            e.startDate = Convert.ToDateTime(query[5]);
                            e.endDate = Convert.ToDateTime(query[6]);
                            e.memberOnly = Convert.ToBoolean(query[7]);
                            e.GameID = Convert.ToInt32(query[8]);
                            records.Add(e);
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.name = query[2].ToString();
                            records.Add(g);
                        }

                        //question record
                        else if (Table == "Question")
                        {
                            Question q = new Question();
                            q.Id = Convert.ToInt32(query[0]);
                            q.hidden = Convert.ToBoolean(query[1]);
                            q.name = query[2].ToString();
                            records.Add(q);
                        }

                        //answer record
                        else if (Table == "Answer")
                        {
                            Answer a = new Answer();
                            a.Id = Convert.ToInt32(query[0]);
                            a.hidden = Convert.ToBoolean(query[1]);
                            a.name = query[2].ToString();
                            a.correct = Convert.ToBoolean(query[3]);
                            a.QuestionID = Convert.ToInt32(query[4]);
                            records.Add(a);
                        }

                        //game question record
                        else if (Table == "GameQuestion")
                        {
                            GameQuestion gq = new GameQuestion();
                            gq.Id = Convert.ToInt32(query[0]);
                            gq.GameID = Convert.ToInt32(query[1]);
                            gq.QuestionID = Convert.ToInt32(query[2]);
                            records.Add(gq);
                        }

                        //item record
                        else if (Table == "Item")
                        {
                            Item i = new Item();
                            i.Id = Convert.ToInt32(query[0]);
                            i.itemName = query[1].ToString();
                            i.valueType = query[2].ToString();
                            records.Add(i);
                        }

                        //EventItem record
                        else if (Table == "EventItem")
                        {
                            EventItem ei = new EventItem();
                            ei.Id = Convert.ToInt32(query[0]);
                            ei.EventId = Convert.ToInt32(query[1]);
                            ei.ItemId = Convert.ToInt32(query[2]);
                            records.Add(ei);
                        }

                        //AttendanceItem record
                        else if (Table == "AttendanceItem")
                        {
                            AttendanceItem ai = new AttendanceItem();
                            ai.Id = Convert.ToInt32(query[0]);
                            ai.AttendanceId = Convert.ToInt32(query[1]);
                            ai.EventItemId = Convert.ToInt32(query[2]);
                            ai.Answer = Convert.ToInt32(query[3]);
                            records.Add(ai);
                        }

                        //attendance record
                        else if (Table == "Attendance")
                        {
                            Attendance a = new Attendance();
                            a.Id = Convert.ToInt32(query[0]);
                            a.memberNumber = query[1].ToString();
                            a.arriveTime = Convert.ToDateTime(query[2]);
                            a.isMember = Convert.ToBoolean(query[3]);
                            a.phone = query[4].ToString();
                            a.firstName = query[5].ToString();
                            a.lastName = query[6].ToString();
                            a.EventID = Convert.ToInt32(query[7]);
                            records.Add(a);
                        }
                    }

                    //return list records
                    return records;
                }

                //single record
                else if (Id != null)
                    while (query.Read())
                    {
                        //event table
                        if (Table == "Event")
                        {
                            Event e = new Event();
                            e.Id = Convert.ToInt32(query[0]);
                            e.hidden = Convert.ToBoolean(query[1]);
                            e.name = query[2].ToString();
                            e.displayName = query[3].ToString();
                            e.nameAbbrev = query[4].ToString();
                            e.startDate = Convert.ToDateTime(query[5]);
                            e.endDate = Convert.ToDateTime(query[6]);
                            e.memberOnly = Convert.ToBoolean(query[7]);
                            e.GameID = Convert.ToInt32(query[8]);
                            return e;
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.name = query[2].ToString();
                            return g;
                        }

                        //question record
                        else if (Table == "Question")
                        {
                            Question q = new Question();
                            q.Id = Convert.ToInt32(query[0]);
                            q.hidden = Convert.ToBoolean(query[1]);
                            q.name = query[2].ToString();
                            return q;
                        }

                        //answer record
                        else if (Table == "Answer")
                        {
                            Answer a = new Answer();
                            a.Id = Convert.ToInt32(query[0]);
                            a.hidden = Convert.ToBoolean(query[1]);
                            a.name = query[2].ToString();
                            a.correct = Convert.ToBoolean(query[3]);
                            a.QuestionID = Convert.ToInt32(query[4]);
                            return a;
                        }

                        //game question record
                        else if (Table == "GameQuestion")
                        {
                            GameQuestion gq = new GameQuestion();
                            gq.Id = Convert.ToInt32(query[0]);
                            gq.GameID = Convert.ToInt32(query[1]);
                            gq.QuestionID = Convert.ToInt32(query[2]);
                            return gq;
                        }

                        //item record
                        else if (Table == "Item")
                        {
                            Item i = new Item();
                            i.Id = Convert.ToInt32(query[0]);
                            i.itemName = query[1].ToString();
                            i.valueType = query[2].ToString();
                            return i;
                        }

                        //EventItem record
                        else if (Table == "EventItem")
                        {
                            EventItem ei = new EventItem();
                            ei.Id = Convert.ToInt32(query[0]);
                            ei.EventId = Convert.ToInt32(query[1]);
                            ei.ItemId = Convert.ToInt32(query[2]);
                            return ei;
                        }

                        //AttendanceItem record
                        else if (Table == "AttendanceItem")
                        {
                            AttendanceItem ai = new AttendanceItem();
                            ai.Id = Convert.ToInt32(query[0]);
                            ai.AttendanceId = Convert.ToInt32(query[1]);
                            ai.EventItemId = Convert.ToInt32(query[2]);
                            ai.Answer = Convert.ToInt32(query[3]);
                            return ai;
                        }

                        //attendance record
                        else if (Table == "Attendance")
                        {
                            Attendance a = new Attendance();
                            a.Id = Convert.ToInt32(query[0]);
                            a.memberNumber = query[1].ToString();
                            a.arriveTime = Convert.ToDateTime(query[2]);
                            a.isMember = Convert.ToBoolean(query[3]);
                            a.phone = query[4].ToString();
                            a.firstName = query[5].ToString();
                            a.lastName = query[6].ToString();
                            a.EventID = Convert.ToInt32(query[7]);
                            return a;
                        }
                    }

                //close connection
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            //catch errors
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }

            //return nothing if invalid parameters
            return null;
        }

        //insert a new record
        public static async Task<int> Insert(dynamic record)
        {
            //use api if available
            if (api.CheckConnection())
                return await api.POST(record);

            //verify the database exists
            Verify();

            //setup table, fields, values, and new id
            string table = "";
            string fields = "";
            string values = "";
            int Id = -1;

            //event record
            if (record.GetType() == typeof(Event))
            {
                table = "Event";
                fields = "hidden, name, displayName, nameAbbrev, startDate, endDate, memberOnly, GameID";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}', '{record.displayName}', '{record.nameAbbrev}', '{record.startDate}', '{record.endDate}', " +
                    $"{Convert.ToInt32(record.memberOnly)}, {record.GameID}";
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                fields = "hidden, name";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}'";
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                fields = "hidden, name";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}'";
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                fields = "hidden, name, correct, QuestionID";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}', {Convert.ToInt32(record.correct)}, {record.QuestionID}";
            }

            //game question record
            else if (record.GetType() == typeof(GameQuestion))
            {
                table = "GameQuestion";
                fields = "GameID, QuestionID";
                values = $"{record.GameID}, {record.QuestionID}";
            }

            //item record
            else if (record.GetType() == typeof(Item))
            {
                table = "Item";
                fields = "itemName, valueType";
                values = $"'{record.itemName}', '{record.valueType}'";
            }

            //EventItem record
            else if (record.GetType() == typeof(EventItem))
            {
                table = "EventItem";
                fields = "EventId, ItemId";
                values = $"{Convert.ToInt32(record.EventId)}, {Convert.ToInt32(record.ItemId)}";
            }

            //AttendanceItem record
            else if (record.GetType() == typeof(AttendanceItem))
            {
                table = "AttendanceItem";
                fields = "AttendanceId, EventItemId, answer";
                values = $"{Convert.ToInt32(record.AttendanceId)}, {Convert.ToInt32(record.EventItemId)}, '{record.Answer}'";
            }

            //attendance record
            else if (record.GetType() == typeof(Attendance))
            {
                table = "Attendance";
                fields = "memberNumber, arriveTime, isMember, phone, firstName, lastName, EventID";
                values = $"'{record.memberNumber}', '{record.arriveTime}', {Convert.ToInt32(record.isMember)}, '{record.phone}', '{record.firstName}', '{record.lastName}', {record.EventID}";
            }

            //table doesn't exist
            else
                return Id;

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //setup insert command
                SqliteCommand cmd = new SqliteCommand($"INSERT INTO {table} ({fields}) VALUES ({values}); SELECT last_insert_rowid();", con);

                //insert record and return id
                Id = Convert.ToInt32(cmd.ExecuteScalar());

                //close connection
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            //catch errors
            catch (Exception ex)
            {
                if (table == "Event" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That event already exists!").ShowAsync();
                else if (table == "Attendance" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("Invalid event!").ShowAsync();
                else
                    await new MessageDialog(ex.Message).ShowAsync();
            }

            return Id;
        }

        //edit a record
        public static async void Update(dynamic record)
        {
            //use api if available
            if (api.CheckConnection())
            {
                await api.PUT(record);
                return;
            }

            //verify the database exists
            Verify();

            //setup table and conditions
            string table = "";
            string conditions = "";

            //event record
            if (record.GetType() == typeof(Event))
            {
                table = "Event";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}', displayName = '{record.displayName}', nameAbbrev = '{record.nameAbbrev}', " +
                    $"startDate = '{record.startDate}', endDate = '{record.endDate}', memberOnly = {Convert.ToInt32(record.memberOnly)}, GameID = {record.GameID}";
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}'";
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}'";
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}', correct = {Convert.ToInt32(record.correct)}, QuestionID = {record.QuestionID}";
            }

            //event game record
            else if (record.GetType() == typeof(GameQuestion))
            {
                table = "GameQuestion";
                conditions = $"GameID = {record.GameID}, QuestionID = {record.QuestionID}";
            }

            //item record
            else if (record.GetType() == typeof(Item))
            {
                table = "Item";
                conditions = $"itemName = '{record.itemName}', valueType = '{record.valueType}'";
            }

            //EventItem record
            else if (record.GetType() == typeof(EventItem))
            {
                table = "EventItem";
                conditions = $"EventId = {Convert.ToInt32(record.EventId)}, ItemId = {Convert.ToInt32(record.ItemId)}";
            }

            //AttendanceItem record
            else if (record.GetType() == typeof(AttendanceItem))
            {
                table = "AttendanceItem";
                conditions = $"AttendanceId = {Convert.ToInt32(record.AttendanceId)}, EventItemId = {Convert.ToInt32(record.EventItemId)}, Answer = '{record.Answer}'";
            }

            //attendance record
            else if (record.GetType() == typeof(Attendance))
            {
                table = "Attendance";
                conditions = $"memberNumber = '{record.memberNumber}', arriveTime = '{record.arriveTime}', isMember = {Convert.ToInt32(record.isMember)}, phone = '{record.phone}', firstName = '{record.firstName}', " +
                    $"lastName = '{record.lastName}', EventID = {record.EventID}";
            }

            //table doesn't exist
            else
                return;

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //setup update command
                SqliteCommand cmd = new SqliteCommand($"UPDATE {table} SET {conditions} WHERE Id = {record.Id};", con);

                //edit record
                cmd.ExecuteNonQuery();

                //close connection
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            //catch errors
            catch (Exception ex)
            {
                //foreign key constraints
                if (table == "Game" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose an event!").ShowAsync();
                else if (table == "Question" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose a game!").ShowAsync();
                else if (table == "Answer" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose a question!").ShowAsync();
                else if (table == "Attendance" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("Invalid event!").ShowAsync();

                //unique constraints
                else if (table == "Event" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That event already exists!").ShowAsync();
                else if (table == "Game" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That game already exists!").ShowAsync();
                else if (table == "Question" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That question already exists!").ShowAsync();
                else if (table == "Answer" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That answer already exists!").ShowAsync();

                //other issue
                else
                    await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        //edit a record
        public static async void Delete(dynamic record)
        {
            //use api if available
            if (api.CheckConnection())
            {
                var res = await api.DELETE(record);
                if (res != "Deleted")
                    await new MessageDialog(res).ShowAsync();
                return;
            }

            //verify the database exists
            Verify();

            //setup table
            string table = "";

            //event record
            if (record.GetType() == typeof(Event))
                table = "Event";

            //game record
            else if (record.GetType() == typeof(Game))
                table = "Game";

            //question record
            else if (record.GetType() == typeof(Question))
                table = "Question";

            //answer record
            else if (record.GetType() == typeof(Answer))
                table = "Answer";

            //event game record
            else if (record.GetType() == typeof(GameQuestion))
                table = "GameQuestion";

            //item record
            else if (record.GetType() == typeof(Item))
                table = "Item";

            //eventItem record
            else if (record.GetType() == typeof(EventItem))
                table = "EventItem";

            //attendanceItem record
            else if (record.GetType() == typeof(AttendanceItem))
                table = "AttendanceItem";

            //attendance record
            else if (record.GetType() == typeof(Attendance))
                table = "Attendance";

            //table doesn't exist
            else
                return;

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //setup update command
                SqliteCommand cmd = new SqliteCommand($"UPDATE {table} SET hidden = 1 WHERE Id = {record.Id};", con);

                //delete many to many relationship
                if (table == "GameQuestion")
                    cmd = new SqliteCommand($"DELETE FROM {table} WHERE Id = {record.Id};", con);

                //edit record
                cmd.ExecuteNonQuery();

                //close connection
                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            //catch errors
            catch (Exception ex)
            {
                //foreign key constraints
                if (table == "Game" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose an event!").ShowAsync();
                else if (table == "Question" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose a game!").ShowAsync();
                else if (table == "Answer" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("You need to choose a question!").ShowAsync();
                else if (table == "Attendance" && ex.Message.Contains("FOREIGN KEY"))
                    await new MessageDialog("Invalid event!").ShowAsync();

                //unique constraints
                else if (table == "Event" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That event already exists!").ShowAsync();
                else if (table == "Game" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That game already exists!").ShowAsync();
                else if (table == "Question" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That question already exists!").ShowAsync();
                else if (table == "Answer" && ex.Message.Contains("UNIQUE"))
                    await new MessageDialog("That answer already exists!").ShowAsync();

                //other issue
                else
                    await new MessageDialog(ex.Message).ShowAsync();
            }
        }
    }
}
