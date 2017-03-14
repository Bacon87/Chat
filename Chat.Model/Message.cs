using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model
{
    [Serializable]
    public class Message
    {
        public string From { get; set; }
        public string Msg { get; set; }

        private Message() { }

        public Message(string from, string msg) {
            From = from;
            Msg = msg;
        }
    }
}
