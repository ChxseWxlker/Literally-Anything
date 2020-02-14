using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP
{
    public class ApiHelper
    {
        //check for internet connection
        public static bool CheckConnection()
        {
            try
            {
                var web = new WebClient();
                web.OpenRead("http://blank.org/");
                return true;
            }
            catch { return false; }
        }
    }
}
