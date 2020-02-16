using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Attendance
    {
        //attendance properties
        public int Id { get; set; }
        public string memberNumber { get; set; }
        public DateTime arriveTime { get; set; }
        public bool isMember { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int EventID { get; set; }
    }
}
