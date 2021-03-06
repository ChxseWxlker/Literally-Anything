﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Event
    {
        public Event()
        {
            //set default values
            this.hidden = false;
            this.Id = 0;
            this.memberOnly = true;
        }

        //event properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string nameAbbrev { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool memberOnly { get; set; }
        public int GameID { get; set; }
    }
}
