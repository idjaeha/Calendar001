using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar001.Model
{
    [Serializable]
    public class Setting
    {
        private string fontFamilyName;
        private string fontSize;
        private string fontStyle;
        private string fontWeight;
        private string backGround;
        private string memoForeGround;
        private string optionForeGround;
        private double width = -1;
        private double height = -1;
        private double top = -1;
        private double left = -1;
        [NonSerialized]
        private TextDecorationCollection textDecorations;

        public string FontFamilyName
        {
            get
            {
                if (fontFamilyName == null)
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
        public string FontSize
        {
            get
            {
                if (fontSize == null)
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
        public string FontStyle
        {
            get
            {
                if (fontStyle == null)
                {
                    fontStyle = System.Windows.FontStyles.Normal.ToString();
                }
                return fontStyle;
            }
            set
            {
                fontStyle = value;
            }
        }
        public string FontWeight
        {
            get
            {
                if (fontWeight == null)
                {
                    fontWeight = System.Windows.FontWeights.Normal.ToString();
                }
                return fontWeight;
            }
            set
            {
                fontWeight = value;
            }
        }
        public string Background
        {
            get
            {
                if (backGround == null)
                {
                    backGround = "#CC000000";
                }
                return backGround;
            }
            set
            {
                backGround = value;
            }
        }
        public string MemoForeground
        {
            get
            {
                if (memoForeGround == null)
                {
                    memoForeGround = System.Windows.Media.Colors.White.ToString();
                }
                return memoForeGround;
            }
            set
            {
                memoForeGround = value;
            }
        }
        public string OptionForeground
        {
            get
            {
                if (optionForeGround == null)
                {
                    optionForeGround = System.Windows.Media.Colors.White.ToString();
                }
                return optionForeGround;
            }
            set
            {
                optionForeGround = value;
            }
        }
        public TextDecorationCollection GetTextDecorations()
        {
            return textDecorations;
        }
        public void SetTextDecorations(TextDecorationCollection textDecorations)
        {
            this.textDecorations = textDecorations;
        }
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }
        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }
        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }
    }
}
