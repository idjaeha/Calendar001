using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                    fontFamilyName = System.Drawing.SystemFonts.DefaultFont.Name;
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
        public System.Windows.FontStyle FontStyle { get; set; }
        public System.Windows.FontWeight FontWeight { get; set; }

        public System.Windows.Media.Brush Background { get; set; }
        public System.Windows.Media.Brush MemoForeground { get; set; }
        public System.Windows.Media.Brush OptionForeground { get; set; }
        public TextDecorationCollection TextDecoration { get; internal set; }
    }
}
