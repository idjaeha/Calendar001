using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarWPF.Model
{
    [Serializable]
    public class Setting
    {
        private string fontFamilyName;
        private string fontSize;

        public string FontFamilyName { get
            {
                if(fontFamilyName == null)
                {
                    fontFamilyName = SystemFonts.DefaultFont.Name;
                }

                return fontFamilyName;
            }
            set
            {
                fontFamilyName = value;
            }
        }
        public string FontSize { get
            {
                if(fontSize == null)
                {
                    fontSize = "15";
                }
                return fontSize;
            }
            set
            {
                fontSize = value;
            }
        }
        public FontStyle FontStyle { get; set; }

        public System.Windows.Media.Brush Background { get; set; }
    }
}
