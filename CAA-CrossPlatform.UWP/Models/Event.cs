using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Event
    {
        public Event()
        {
            //set default values
            this.hidden = false;
            this.memberOnly = true;
        }

        //event properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string nameAbbrev { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool memberOnly { get; set; }

        //event methods
        /*
        public static List<Event> GetAll()
        {
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "EventsDB.db");
            using (SqliteConnection con = new SqliteConnection($"Filename={path}"))
            {
                try
                {
                    con.Open();
                    SqliteCommand cmd = new SqliteCommand("SELECT * FROM Event", con);
                    SqliteDataReader query = cmd.ExecuteReader();

                    while (query.Read())
                    {
                        ev.Id = Convert.ToInt32(query[0]);
                        ev.hidden = Convert.ToBoolean(query[1]);
                        ev.name = query[2].ToString();
                        ev.nameAbbrev = query[3].ToString();
                        ev.startDate = Convert.ToDateTime(query[4]);
                        ev.endDate = Convert.ToDateTime(query[5]);
                        ev.memberOnly = Convert.ToBoolean(query[6]);
                        ev.trackGuestNum = Convert.ToInt32(query[7]);
                        ev.trackAdultNum = Convert.ToInt32(query[8]);
                        ev.trackChildNum = Convert.ToInt32(query[9]);
                    }

                    con.Close();
                }

                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message).ShowAsync();
                }
            }
            */
    }

    public class RootEvent
    {
        public List<Event> events { get; set; }
    }
}
