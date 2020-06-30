using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Model
{
    public class MenuItemWithID : System.Windows.Forms.MenuItem
    {
        public string ID { get; set; }
        public MenuItemWithID() : base() { }
    }
}
