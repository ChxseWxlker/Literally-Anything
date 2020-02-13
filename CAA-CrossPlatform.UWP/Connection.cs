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

        //returns a specific record or all records
        public static async Task<dynamic> Get(string Table, int? ID = null)
        {
            //try connecting to the database
            try
            {
                //open connection
                con.Open();

                //select all records
                SqliteCommand cmd = new SqliteCommand($"SELECT * FROM {Table};", con);

                //select single record
                if (ID != null)
                    cmd = new SqliteCommand($"SELECT * FROM {Table} WHERE Id = {ID};", con);

                //query record(s)
                SqliteDataReader query = cmd.ExecuteReader();

                //all records
                if (ID == null)
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

                    while (query.Read())
                    {
                        //event table
                        if (Table == "Event")
                        {
                            Event e = new Event();
                            e.Id = Convert.ToInt32(query[0]);
                            e.hidden = Convert.ToBoolean(query[1]);
                            e.name = query[2].ToString();
                            e.nameAbbrev = query[3].ToString();
                            e.startDate = Convert.ToDateTime(query[4]);
                            e.endDate = Convert.ToDateTime(query[5]);
                            e.memberOnly = Convert.ToBoolean(query[6]);
                            records.Add(e);
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.title = query[2].ToString();
                            g.EventID = Convert.ToInt32(query[3]);
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
                    }

                    //return list records
                    return records;
                }

                //single record
                else if (ID != null)
                    while (query.Read())
                    {
                        //event table
                        if (Table == "Event")
                        {
                            Event e = new Event();
                            e.Id = Convert.ToInt32(query[0]);
                            e.hidden = Convert.ToBoolean(query[1]);
                            e.name = query[2].ToString();
                            e.nameAbbrev = query[3].ToString();
                            e.startDate = Convert.ToDateTime(query[4]);
                            e.endDate = Convert.ToDateTime(query[5]);
                            e.memberOnly = Convert.ToBoolean(query[6]);
                            return e;
                        }

                        //game table
                        else if (Table == "Game")
                        {
                            Game g = new Game();
                            g.Id = Convert.ToInt32(query[0]);
                            g.hidden = Convert.ToBoolean(query[1]);
                            g.title = query[2].ToString();
                            g.EventID = Convert.ToInt32(query[3]);
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
            //get table, fields, and values, and new id
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
                fields = "hidden, title, EventID";
                values = $"{Convert.ToInt32(record.hidden)}, '{record.title}', {record.EventID}";
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
    }
}
