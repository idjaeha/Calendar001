using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Src
{
    class Day
    {
        DateTime date;
        private Memo _memo;
        public Memo Memo
        {
            get
            {
                if (_memo == null)
                {
                    return new Memo();
                }
                else
                {
                    return _memo;
                }
            }
            set
            {
                if (_memo == null)
                {
                    _memo = new Memo();
                }
            }
        }
    }
}
