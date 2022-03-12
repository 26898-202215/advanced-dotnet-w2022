using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Week08_1_SignalREx.Models
{
    public class ChatMessage
    {
        [Key]
        public Guid Id{ get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
