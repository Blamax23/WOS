using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class MailConfiguration
    {
        public string? SenderMail { get; set; }
        public string? SenderPassword { get; set; }
        public string? ReceiverMail { get; set; }
        public string? SmtpServer { get; set; }

    }
}
