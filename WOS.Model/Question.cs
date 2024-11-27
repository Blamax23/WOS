using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOS.Model
{
    public class Question
    {
        public int Id { get; set; }
        public string Intitule { get; set; }
        public string Reponse { get; set; }
    }
}
