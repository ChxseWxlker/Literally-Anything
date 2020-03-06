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
        private async static Task<int> Verify()
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
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE 'Game' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL UNIQUE);" +

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

                    //create item table
                    "CREATE TABLE 'Item' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'name' TEXT NOT NULL, 'valueType' TEXT);" +

                    //create event item table
                    "CREATE TABLE 'EventItem' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'EventId' INTEGER NOT NULL, 'ItemId' INTEGER NOT NULL, " +
                    "FOREIGN KEY('EventId') REFERENCES 'Event'('Id'), FOREIGN KEY('ItemId') REFERENCES 'Item'('Id'));" +

                    //create attendance item table
                    "CREATE TABLE 'AttendanceItem' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'AttendanceId' INTEGER NOT NULL, 'EventItemId' INTEGER NOT NULL, " +
                    "'input' TEXT NOT NULL, FOREIGN KEY('AttendanceId') REFERENCES 'Attendance'('Id'), FOREIGN KEY('EventItemId') REFERENCES 'EventItem'('Id'));" +

                    //create attendance table
                    "CREATE TABLE 'Attendance' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'memberNumber' TEXT, 'arriveTime' TEXT NOT NULL, 'isMember' INTEGER, " +
                    "'phone' TEXT, 'firstName' TEXT, 'lastName' TEXT, 'EventID' INTEGER NOT NULL, FOREIGN KEY('EventID') REFERENCES 'Event'('Id'));", con);

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

            return 0;
        }

        //returns a specific record or all records
        public static async Task<dynamic> Get(string Table, int? Id = null)
        {
            //verify the database exists
            await Verify();

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //select all records
                SqliteCommand cmd = new SqliteCommand($"SELECT * FROM {Table};", con);

                //select single record
                if (Id != null)
                {
                    cmd = new SqliteCommand($"SELECT * FROM {Table} WHERE Id = @id;", con);
                    cmd.Parameters.AddWithValue("@id", Id);
                }

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
                            i.name = query[1].ToString();
                            i.valueType = query[2].ToString();
                            records.Add(i);
                        }

                        //Event item record
                        else if (Table == "EventItem")
                        {
                            EventItem ei = new EventItem();
                            ei.Id = Convert.ToInt32(query[0]);
                            ei.EventId = Convert.ToInt32(query[1]);
                            ei.ItemId = Convert.ToInt32(query[2]);
                            records.Add(ei);
                        }

                        //Attendance item record
                        else if (Table == "AttendanceItem")
                        {
                            AttendanceItem ai = new AttendanceItem();
                            ai.Id = Convert.ToInt32(query[0]);
                            ai.AttendanceId = Convert.ToInt32(query[1]);
                            ai.EventItemId = Convert.ToInt32(query[2]);
                            ai.input = Convert.ToInt32(query[3]);
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
                            i.name = query[1].ToString();
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
                            ai.input = Convert.ToInt32(query[3]);
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
            //verify the database exists
            await Verify();

            //setup table, fields, values, and new id
            string table = "";
            string fields = "";
            List<SqliteParameter> parameters = new List<SqliteParameter>();
            int Id = -1;

            //event record
            if (record.GetType() == typeof(Event))
            {
                table = "Event";
                fields = "hidden, name, displayName, nameAbbrev, startDate, endDate, memberOnly, GameID";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@displayName", record.displayName));
                parameters.Add(new SqliteParameter("@nameAbbrev", record.nameAbbrev));
                parameters.Add(new SqliteParameter("@startDate", record.startDate));
                parameters.Add(new SqliteParameter("@endDate", record.endDate));
                parameters.Add(new SqliteParameter("@memberOnly", Convert.ToInt32(record.memberOnly)));
                parameters.Add(new SqliteParameter("@GameID", record.GameID));
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                fields = "hidden, name";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                fields = "hidden, name";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                fields = "hidden, name, correct, QuestionID";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@correct", Convert.ToInt32(record.correct)));
                parameters.Add(new SqliteParameter("@QuestionID", record.QuestionID));
            }

            //game question record
            else if (record.GetType() == typeof(GameQuestion))
            {
                table = "GameQuestion";
                fields = "GameID, QuestionID";

                parameters.Add(new SqliteParameter("@GameID", record.GameID));
                parameters.Add(new SqliteParameter("@QuestionID", record.QuestionID));
            }

            //item record
            else if (record.GetType() == typeof(Item))
            {
                table = "Item";
                fields = "name, valueType";

                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@valueType", record.valueType));
            }

            //EventItem record
            else if (record.GetType() == typeof(EventItem))
            {
                table = "EventItem";
                fields = "EventId, ItemId";

                parameters.Add(new SqliteParameter("@EventId", record.EventId));
                parameters.Add(new SqliteParameter("@ItemId", record.ItemId));
            }

            //AttendanceItem record
            else if (record.GetType() == typeof(AttendanceItem))
            {
                table = "AttendanceItem";
                fields = "AttendanceId, EventItemId, input";

                parameters.Add(new SqliteParameter("@AttendanceId", record.AttendanceId));
                parameters.Add(new SqliteParameter("@EventItemId", record.EventItemId));
                parameters.Add(new SqliteParameter("@input", record.input));
            }

            //attendance record
            else if (record.GetType() == typeof(Attendance))
            {
                table = "Attendance";
                fields = "memberNumber, arriveTime, isMember, phone, firstName, lastName, EventID";

                if (string.IsNullOrEmpty(record.memberNumber))
                    parameters.Add(new SqliteParameter("@memberNumber", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@memberNumber", record.memberNumber));

                if (string.IsNullOrEmpty(record.phone))
                    parameters.Add(new SqliteParameter("@phone", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@phone", record.phone));

                if (string.IsNullOrEmpty(record.firstName))
                    parameters.Add(new SqliteParameter("@firstName", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@firstName", record.firstName));

                if (string.IsNullOrEmpty(record.lastName))
                    parameters.Add(new SqliteParameter("@lastName", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@lastName", record.lastName));

                parameters.Add(new SqliteParameter("@arriveTime", record.arriveTime));
                parameters.Add(new SqliteParameter("@isMember", Convert.ToInt32(record.isMember)));
                parameters.Add(new SqliteParameter("@EventID", record.EventID));
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
                SqliteCommand cmd = new SqliteCommand($"INSERT INTO {table} ({fields}) VALUES ({fields.Insert(0, "@").Replace(", ", ", @")}); SELECT last_insert_rowid();", con);

                //add parameters
                foreach (SqliteParameter parameter in parameters)
                    cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);

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
            //verify the database exists
            await Verify();

            //setup table and conditions
            string table = "";
            string conditions = "";
            List<SqliteParameter> parameters = new List<SqliteParameter>();

            //event record
            if (record.GetType() == typeof(Event))
            {
                table = "Event";
                conditions = "hidden = @hidden, name = @name, displayName = @displayName, nameAbbrev = @nameAbbrev, " +
                    $"startDate = @startDate, endDate = @endDate, memberOnly = @memberOnly, GameID = @GameID";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@displayName", record.displayName));
                parameters.Add(new SqliteParameter("@nameAbbrev", record.nameAbbrev));
                parameters.Add(new SqliteParameter("@startDate", record.startDate));
                parameters.Add(new SqliteParameter("@endDate", record.endDate));
                parameters.Add(new SqliteParameter("@memberOnly", Convert.ToInt32(record.memberOnly)));
                parameters.Add(new SqliteParameter("@GameID", record.GameID));
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                conditions = $"hidden = @hidden, name = @name";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                conditions = $"hidden = @hidden, name = @name";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                conditions = $"hidden = @hidden, name = @name, correct = @correct, QuestionID = @QuestionID";

                parameters.Add(new SqliteParameter("@hidden", Convert.ToInt32(record.hidden)));
                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@correct", Convert.ToInt32(record.correct)));
                parameters.Add(new SqliteParameter("@QuestionID", record.QuestionID));
            }

            //event game record
            else if (record.GetType() == typeof(GameQuestion))
            {
                table = "GameQuestion";
                conditions = $"GameID = @GameID, QuestionID = @QuestionID";

                parameters.Add(new SqliteParameter("@GameID", record.GameID));
                parameters.Add(new SqliteParameter("@QuestionID", record.QuestionID));
            }

            //item record
            else if (record.GetType() == typeof(Item))
            {
                table = "Item";
                conditions = $"name = @name, valueType = @valueType";

                parameters.Add(new SqliteParameter("@name", record.name));
                parameters.Add(new SqliteParameter("@valueType", record.valueType));
            }

            //EventItem record
            else if (record.GetType() == typeof(EventItem))
            {
                table = "EventItem";
                conditions = $"EventId = @EventId, ItemId = @ItemId";

                parameters.Add(new SqliteParameter("@EventId", record.EventId));
                parameters.Add(new SqliteParameter("@ItemId", record.ItemId));
            }

            //AttendanceItem record
            else if (record.GetType() == typeof(AttendanceItem))
            {
                table = "AttendanceItem";
                conditions = $"AttendanceId = @AttendanceId, EventItemId = @EventItemId, input = @input";

                parameters.Add(new SqliteParameter("@AttendanceId", record.AttendanceId));
                parameters.Add(new SqliteParameter("@EventItemId", record.EventItemId));
                parameters.Add(new SqliteParameter("@input", record.input));
            }

            //attendance record
            else if (record.GetType() == typeof(Attendance))
            {
                table = "Attendance";
                conditions = $"memberNumber = @memberNumber, arriveTime = @arriveTime, isMember = @isMember, phone = @phone, firstName = @firstName, " +
                    $"lastName = @lastName, EventID = @EventID";

                if (string.IsNullOrEmpty(record.memberNumber))
                    parameters.Add(new SqliteParameter("@memberNumber", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@memberNumber", record.memberNumber));

                if (string.IsNullOrEmpty(record.phone))
                    parameters.Add(new SqliteParameter("@phone", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@phone", record.phone));

                if (string.IsNullOrEmpty(record.firstName))
                    parameters.Add(new SqliteParameter("@firstName", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@firstName", record.firstName));

                if (string.IsNullOrEmpty(record.lastName))
                    parameters.Add(new SqliteParameter("@lastName", DBNull.Value));
                else
                    parameters.Add(new SqliteParameter("@lastName", record.lastName));

                parameters.Add(new SqliteParameter("@arriveTime", record.arriveTime));
                parameters.Add(new SqliteParameter("@isMember", Convert.ToInt32(record.isMember)));
                parameters.Add(new SqliteParameter("@EventID", record.EventID));
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
                SqliteCommand cmd = new SqliteCommand($"UPDATE {table} SET {conditions} WHERE Id = @id;", con);

                //add parameters
                cmd.Parameters.AddWithValue("@id", record.Id);
                foreach (SqliteParameter parameter in parameters)
                    cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);

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
        public static async Task<int> Delete(dynamic record)
        {
            //verify the database exists
            await Verify();

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
                return 0;

            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //setup update command
                SqliteCommand cmd = new SqliteCommand($"UPDATE {table} SET hidden = @hidden WHERE Id = @id;", con);

                //delete many to many relationship
                if (table == "GameQuestion" || table == "EventItem" || table == "AttendanceItem" || table == "Item")
                    cmd = new SqliteCommand($"DELETE FROM {table} WHERE Id = @id;", con);

                //add parameters
                cmd.Parameters.AddWithValue("@hidden", 1);
                cmd.Parameters.AddWithValue("@id", record.Id);

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

            return 0;
        }
    }
}
