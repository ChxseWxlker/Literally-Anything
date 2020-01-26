using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP
{
    public class Game
    {
        public int id { get; set; }
        public int[] questions { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public string name { get; set; }
        public string[] answers { get; set; }
        private int answerIndex { get; set; }
        public string correct
        {
            get
            {
                return answers[answerIndex];
            }
            set
            {
                for (int i = 0; i < answers.Length; i++)
                {
                    if (answers[i].ToLower() == value.ToLower())
                        answerIndex = i;
                }
            }
        }
    }
}
