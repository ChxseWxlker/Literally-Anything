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

                    //create event table
                    SqliteCommand cmd = new SqliteCommand("CREATE TABLE 'Event' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, " +
                    "'name' TEXT NOT NULL, 'displayName' TEXT NOT NULL, 'nameAbbrev' TEXT NOT NULL UNIQUE, 'startDate' TEXT NOT NULL, 'endDate' TEXT NOT NULL, 'memberOnly' " +
                    "INTEGER NOT NULL DEFAULT 1 );" +

                    //create game table
                    "CREATE TABLE 'Game' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'title' TEXT NOT NULL UNIQUE, " +
                    "'EventID' INTEGER NOT NULL, FOREIGN KEY('EventID') REFERENCES 'Event'('Id') );" +

                    //create question table
                    "CREATE TABLE 'Question' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL, 'GameID' " +
                    "INTEGER NOT NULL, FOREIGN KEY('GameID') REFERENCES 'Game'('Id') );" +

                    //create answer table
                    "CREATE TABLE 'Answer' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'hidden' INTEGER NOT NULL DEFAULT 0, 'name' TEXT NOT NULL, 'correct' " +
                    "INTEGER NOT NULL, 'QuestionID' INTEGER NOT NULL, FOREIGN KEY('QuestionID') REFERENCES 'Question'('Id') );" +

                    //create event game table
                    "CREATE TABLE 'EventGame' ( 'Id' INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 'EventID' INTEGER NOT NULL, 'GameID' INTEGER NOT NULL, " +
                    "FOREIGN KEY('GameID') REFERENCES 'Game'('Id'), FOREIGN KEY('EventID') REFERENCES 'Event'('Id') );", con);

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
                    else if (Table == "EventGame")
                        records = new List<EventGame>();

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
                            records.Add(e);
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.title = query[2].ToString();
                            records.Add(g);
                        }

                        //question record
                        else if (Table == "Question")
                        {
                            Question q = new Question();
                            q.Id = Convert.ToInt32(query[0]);
                            q.hidden = Convert.ToBoolean(query[1]);
                            q.name = query[2].ToString();
                            q.GameID = Convert.ToInt32(query[3]);
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

                        //event game record
                        else if (Table == "EventGame")
                        {
                            EventGame eg = new EventGame();
                            eg.Id = Convert.ToInt32(query[0]);
                            eg.EventID = Convert.ToInt32(query[1]);
                            eg.GameID = Convert.ToInt32(query[2]);
                            records.Add(eg);
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
                            return e;
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.title = query[2].ToString();
                            return g;
                        }

                        //question record
                        else if (Table == "Question")
                        {
                            Question q = new Question();
                            q.Id = Convert.ToInt32(query[0]);
                            q.hidden = Convert.ToBoolean(query[1]);
                            q.name = query[2].ToString();
                            q.GameID = Convert.ToInt32(query[3]);
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

                        //event game record
                        else if (Table == "EventGame")
                        {
                            EventGame eg = new EventGame();
                            eg.Id = Convert.ToInt32(query[0]);
                            eg.EventID = Convert.ToInt32(query[1]);
                            eg.GameID = Convert.ToInt32(query[2]);
                            return eg;
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
                fields = "hidden, name, displayName, nameAbbrev, startDate, endDate, memberOnly";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}', '{record.displayName}', '{record.nameAbbrev}', '{record.startDate}', '{record.endDate}', {Convert.ToInt32(record.memberOnly)}";
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                fields = "hidden, title";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.title}'";
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                fields = "hidden, name, GameID";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}', {record.GameID}";
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                fields = "hidden, name, correct, QuestionID";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.name}', {Convert.ToInt32(record.correct)}, {record.QuestionID}";
            }

            //event game record
            else if (record.GetType() == typeof(EventGame))
            {
                table = "EventGame";
                fields = "EventID, GameID";
                values = $"{record.EventID}, {record.GameID}";
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
                else
                    await new MessageDialog(ex.Message).ShowAsync();
            }

            return Id;
        }

        //edit a record
        public static async void Update(dynamic record)
        {
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
                    $"startDate = '{record.startDate}', endDate = '{record.endDate}', memberOnly = {Convert.ToInt32(record.memberOnly)}";
            }

            //game record
            else if (record.GetType() == typeof(Game))
            {
                table = "Game";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, title = '{record.title}'";
            }

            //question record
            else if (record.GetType() == typeof(Question))
            {
                table = "Question";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}', GameID = {record.GameID}";
            }

            //answer record
            else if (record.GetType() == typeof(Answer))
            {
                table = "Answer";
                conditions = $"hidden = {Convert.ToInt32(record.hidden)}, name = '{record.name}', correct = {Convert.ToInt32(record.correct)}, QuestionID = {record.QuestionID}";
            }

            //event game record
            else if (record.GetType() == typeof(EventGame))
            {
                table = "EventGame";
                conditions = $"EventID = {record.EventID}, GameID = {record.GameID}";
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
