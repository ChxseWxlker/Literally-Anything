using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Question
    {
        public Question()
        {
            //set default values
            this.hidden = false;
        }

        //question properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
    }
}
