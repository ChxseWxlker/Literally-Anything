using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class TrackingInfo
    {
        public TrackingInfo()
        {
            //set default values
            this.hidden = false;
        }

        //tracking info properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string item { get; set; }
        public int amount { get; set; }
        public int EventID { get; set; }
    }
}