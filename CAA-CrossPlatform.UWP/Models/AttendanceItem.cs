using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class AttendanceItem
    {
        public AttendanceItem()
        {
            //set default values
            this.Id = 0;
        }

        //AttendanceItem properties
        public int Id { get; set; }
        public int AttendanceId { get; set; }
        public int EventItemId { get; set; }
        public string input { get; set; }
    }
}
