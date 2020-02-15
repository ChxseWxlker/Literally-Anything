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
        private static bool CheckConnection()
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
            int hash = $"{u.username}{u.password}".GetHashCode() * int.MaxValue;
            if (hash < 0)
                hash *= -1;
            u.apiKey = hash.ToString().Substring(0, 10);

            //convert to json
            string jsonObject = JsonConvert.SerializeObject(u, Formatting.None);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            //try calling api
            try
            {
                var response = await client.PostAsync($"{server}/api/User/", content);
                var responseString = await response.Content.ReadAsStringAsync();

                //set headers
                client.DefaultRequestHeaders.Add("username", username);
                client.DefaultRequestHeaders.Add("password", username);
                client.DefaultRequestHeaders.Add("APIKey", u.apiKey);

                //return welcome message
                return $"Welcome {username}";
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
            //reset headers
            client.DefaultRequestHeaders.Clear();

            //get users
            var res = await client.GetStringAsync($"{server}/api/User/");
            List<User> users = new List<User>();
            JsonConvert.PopulateObject(res, users);

            //check credentials
            foreach (User u in users)
                if (u.username == username && u.password == password)
                {
                    user = u;
                    break;
                }

            //user doesn't exist
            if (user == null)
                return "The account doesn't exist or you entered invalid credentials, please try again.";

            //set headers
            client.DefaultRequestHeaders.Add("username", user.username);
            client.DefaultRequestHeaders.Add("password", user.password);
            client.DefaultRequestHeaders.Add("APIKey", user.apiKey);

            //return welcome message
            return $"Welcome {user.username}";
        }

        //get request
        public async Task<dynamic> GET(string endpoint, int? Id = null)
        {
            //test connection before call
            if (!CheckConnection())
                return null;

            //setup address with endpoint
            string address = $"{server}/api/{endpoint}/{Id}";

            //call api and return object(s)
            var res = await client.GetStringAsync(address);

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

            //error
            else
                return res;

            //populate object with json data
            JsonConvert.PopulateObject(res, jsonObject);

            //return populated object
            return jsonObject;
        }
    }
}