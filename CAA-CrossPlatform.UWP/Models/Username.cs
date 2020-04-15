using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Username
    {
        public Username()
        {
            //set default values
            this.Id = 0;
        }

        //username properties
        public int Id { get; set; }
        public string username { get; set; }
        public string salt { get; set; }
        public string password { get; set; }
        public string apiKey { get; set; }
    }
}
