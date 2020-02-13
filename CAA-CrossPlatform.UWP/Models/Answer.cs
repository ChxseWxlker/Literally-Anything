using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP.Models
{
    public class Answer
    {
        public Answer()
        {
            //set default values
            this.hidden = false;
        }

        //answer properties
        public int Id { get; set; }
        public bool hidden { get; set; }
        public string name { get; set; }
        public bool correct { get; set; }
        public int QuestionID { get; set; }
    }
}
