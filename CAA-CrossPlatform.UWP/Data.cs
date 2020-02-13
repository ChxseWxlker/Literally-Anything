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
    public class Data
    {
        //returns a specific record or all records
        public static async Task<dynamic> GetRecord(string Table, int? ID = null)
        {
            //get path for database
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "EventsDB.db");
            SqliteConnection con = new SqliteConnection($"Filename={path}");

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
                    List<dynamic> records = new List<dynamic>();
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
    }
}
