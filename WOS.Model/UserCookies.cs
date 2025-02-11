using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class UserCookies
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool ConsentGiven { get; set; }
        public DateTime DateGiven { get; set; } = DateTime.UtcNow;
        public string UserIp { get; set; }
        public string UserAgent { get; set; }
    }
}
