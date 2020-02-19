using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CAA_CrossPlatform.UWP.Models;
using Newtonsoft.Json;

namespace CAA_CrossPlatform.UWP
{
    public class ApiHandler
    {
        /*  --endpoints--
            Event
            Game
            Question
            Answer
            EventGame
            GameQuestion
        */

        //setup server reference
        private static readonly string server = "http://eventrestfulapi.azurewebsites.net";

        //setup user
        private static User user = null;

        //setup client
        private static readonly HttpClient client = new HttpClient();

        //check api connection
        public bool CheckConnection()
        {
            try
            {
                var web = new WebClient();
                web.OpenRead(server);
                return true;
            }
            catch { return false; }
        }

        //register
        public async Task<string> Register(string username, string password)
        {
            //reset headers
            client.DefaultRequestHeaders.Clear();

            //create user
            User u = new User();
            u.username = username;
            u.password = password;
            int hash = $"{u.username}{u.password}".GetHashCode();
            if (hash < 0)
                hash *= -1;
            string hashStr = hash.ToString();
            Random rng = new Random();
            for (int i = hashStr.Length; i < 11; i++)
            {
                hashStr += rng.Next(0, 9);
            }
            u.apiKey = hashStr.Substring(0, 10);

            //convert to json
            string jsonObject = JsonConvert.SerializeObject(u, Formatting.None);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            //try calling api
            try
            {
                var response = await client.PostAsync($"{server}/api/User/", content);
                var responseString = response.Content.ReadAsStringAsync();

                //account exists
                if (responseString.Result.Contains("already exists"))
                    return "That username is already in use, please choose another.";

                //invalid username
                else if (responseString.Result.Contains("valid"))
                    return "That is not a valid username, please choose another.";

                //set headers
                client.DefaultRequestHeaders.Add("username", username);
                client.DefaultRequestHeaders.Add("password", username);
                client.DefaultRequestHeaders.Add("APIKey", u.apiKey);

                //return welcome message
                return $"Welcome {u.username}";
            }
            catch (Exception ex)
            {
                //show error message
                return ex.Message;
            }
        }

        //login
        public async Task<string> Login(string username, string password)
        {
            //reset user
            user = null;

            //reset headers
            client.DefaultRequestHeaders.Clear();

            //get users
            List<User> users = await GET("User");

            //check credentials
            foreach (User u in users)
                if (u.username.ToLower() == username.ToLower() && Encryption.CheckHashSalt(password, u.password, u.salt))
                {
                    user = u;
                    break;
                }

            //user doesn't exist
            if (user == null)
                return "That account doesn't exist or you entered invalid credentials, please try again.";

            //set headers
            client.DefaultRequestHeaders.Add("username", user.username);
            client.DefaultRequestHeaders.Add("password", user.password);
            client.DefaultRequestHeaders.Add("APIKey", user.apiKey);

            //return welcome message
            return $"Welcome {user.username}";
        }

        //logout
        public string Logout()
        {
            //reset headers
            client.DefaultRequestHeaders.Clear();

            //return logout message
            return "Logged out";
        }

        //get request
        public async Task<dynamic> GET(string endpoint, int? Id = null)
        {
            //setup address with endpoint
            string address = $"{server}/api/{endpoint}/{Id}";

            //call api and return object(s)
            var res = "";
            try
            {
                res = await client.GetStringAsync(address);
            }
            catch
            {
                return null;
            }

            //setup json object
            dynamic jsonObject = null;

            //event object
            if (endpoint == "Event")
            {
                if (Id == null)
                    jsonObject = new List<Event>();
                else
                    jsonObject = new Event();
            }

            //game object
            else if (endpoint == "Game")
            {
                if (Id == null)
                    jsonObject = new List<Game>();
                else
                    jsonObject = new Game();
            }

            //question object
            else if (endpoint == "Question")
            {
                if (Id == null)
                    jsonObject = new List<Question>();
                else
                    jsonObject = new Question();
            }

            //answer object
            else if (endpoint == "Answer")
            {
                if (Id == null)
                    jsonObject = new List<Answer>();
                else
                    jsonObject = new Answer();
            }

            //game question object
            else if (endpoint == "GameQuestion")
            {
                if (Id == null)
                    jsonObject = new List<GameQuestion>();
                else
                    jsonObject = new GameQuestion();
            }

            //user object
            else if (endpoint == "User")
            {
                if (Id == null)
                    jsonObject = new List<User>();
                else
                    jsonObject = new User();
            }

            //tracking info object
            else if (endpoint == "TrackingInfo")
            {
                if (Id == null)
                    jsonObject = new List<TrackingInfo>();
                else
                    jsonObject = new TrackingInfo();
            }

            //attendance object
            else if (endpoint == "Attendance")
            {
                if (Id == null)
                    jsonObject = new List<Attendance>();
                else
                    jsonObject = new Attendance();
            }

            //error
            else
                return null;

            //populate object with json data
            JsonConvert.PopulateObject(res, jsonObject);

            //return populated object
            return jsonObject;
        }

        //post request
        public async Task<dynamic> POST(dynamic record)
        {
            //setup endpoint
            string endpoint = "";

            //event object
            if (record.GetType() == typeof(Event))
                endpoint = "Event";

            //game object
            else if (record.GetType() == typeof(Game))
                endpoint = "Game";

            //question object
            else if (record.GetType() == typeof(Question))
                endpoint = "Question";

            //answer object
            else if (record.GetType() == typeof(Answer))
                endpoint = "Answer";

            //game question object
            else if (record.GetType() == typeof(GameQuestion))
                endpoint = "GameQuestion";

            //user object
            else if (record.GetType() == typeof(User))
                endpoint = "User";

            //tracking info object
            else if (record.GetType() == typeof(TrackingInfo))
                endpoint = "TrackingInfo";

            //attendance object
            else if (record.GetType() == typeof(Attendance))
                endpoint = "Attendance";

            //error
            else
                return null;

            //setup address with endpoint
            string address = $"{server}/api/{endpoint}/";

            //call api and return response
            var res = new HttpResponseMessage();
            var resStr = "";
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(record, Formatting.None), Encoding.UTF8, "application/json");
                res = await client.PostAsync(address, content);
                resStr = await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            //return message
            return Convert.ToInt32(resStr);
        }

        //put request
        public async Task<dynamic> PUT(dynamic record)
        {
            //test connection before call
            if (!CheckConnection())
                return null;

            //setup endpoint
            string endpoint = "";

            //event object
            if (record.GetType() == typeof(Event))
                endpoint = "Event";

            //game object
            else if (record.GetType() == typeof(Game))
                endpoint = "Game";

            //question object
            else if (record.GetType() == typeof(Question))
                endpoint = "Question";

            //answer object
            else if (record.GetType() == typeof(Answer))
                endpoint = "Answer";

            //game question object
            else if (record.GetType() == typeof(GameQuestion))
                endpoint = "GameQuestion";

            //user object
            else if (record.GetType() == typeof(User))
                endpoint = "User";

            //tracking info object
            else if (record.GetType() == typeof(TrackingInfo))
                endpoint = "TrackingInfo";

            //attendance object
            else if (record.GetType() == typeof(Attendance))
                endpoint = "Attendance";

            //error
            else
                return null;

            //setup address with endpoint
            string address = $"{server}/api/{endpoint}/";

            //call api and return response
            var res = new HttpResponseMessage();
            var resStr = "";
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(record, Formatting.None), Encoding.UTF8, "application/json");
                res = await client.PutAsync(address, content);
                resStr = await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            //return message
            return resStr;
        }

        //delete request
        public async Task<dynamic> DELETE(dynamic record)
        {
            //setup endpoint
            string endpoint = "";

            //event object
            if (record.GetType() == typeof(Event))
                endpoint = "Event";

            //game object
            else if (record.GetType() == typeof(Game))
                endpoint = "Game";

            //question object
            else if (record.GetType() == typeof(Question))
                endpoint = "Question";

            //answer object
            else if (record.GetType() == typeof(Answer))
                endpoint = "Answer";

            //game question object
            else if (record.GetType() == typeof(GameQuestion))
                endpoint = "GameQuestion";

            //user object
            else if (record.GetType() == typeof(User))
                endpoint = "User";

            //tracking info object
            else if (record.GetType() == typeof(TrackingInfo))
                endpoint = "TrackingInfo";

            //attendance object
            else if (record.GetType() == typeof(Attendance))
                endpoint = "Attendance";

            //error
            else
                return null;

            //setup address with endpoint
            string address = $"{server}/api/{endpoint}/{record.Id}";

            //call api and return response
            var res = new HttpResponseMessage();
            var resStr = "";
            try
            {
                res = await client.DeleteAsync(address);
                resStr = await res.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            //return message
            return resStr;
        }
    }
}