using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CalendarWPF.Controller
{
    public class ConvertHelper
    {
        private static BrushConverter cBrush;
        public static BrushConverter CBrush
        {
            get
            {
                if(cBrush == null)
                {
                    cBrush = new BrushConverter();
                }
                return cBrush;
            }
        }

        private static FontStyleConverter cFontStyle;
        public static FontStyleConverter CFontStyle
        {
            get
            {
                if (cFontStyle == null)
                {
                    cFontStyle = new FontStyleConverter();
                }
                return cFontStyle;
            }
        }

        private static FontWeightConverter cFontWeight;
        public static FontWeightConverter CFontWeight
        {
            get
            {
                if (cFontWeight == null)
                {
                    cFontWeight = new FontWeightConverter();
                }
                return cFontWeight;
            }
        }
    }
}
