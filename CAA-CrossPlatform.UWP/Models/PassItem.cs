using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class PassItem
    {
        public static Answer environmentAnswer { get; set; }
        public static Attendance environmentAttendance { get; set; }
        public static AttendanceItem environmentAttendanceItem { get; set; }
        public static Event environmentEvent { get; set; }
        public static EventItem environmentEventItem { get; set; }
        public static Game environmentGame { get; set; }
        public static GameQuestion environmentGameQuestion { get; set; }
        public static Item environmentItem { get; set; }
        public static Question environmentQuestion { get; set; }
    }
}
