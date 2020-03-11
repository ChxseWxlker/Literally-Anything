using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class EventItem
    {
        public EventItem()
        {
            //set default values
            this.Id = 0;
        }

        //EventItem properties
        public int Id { get; set; }
        public int EventId { get; set; }
        public int ItemId { get; set; }
    }
}
