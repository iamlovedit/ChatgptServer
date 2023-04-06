using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRWPF.Models
{
    public class ChatMessage
    {
        public string Content { get; }

        public ChatMessage(string content)
        {
            Content = content;
        }
    }
}
