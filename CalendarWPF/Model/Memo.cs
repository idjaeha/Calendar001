using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Src
{
    [Serializable]
    public class Memo
    {
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
    }
}
