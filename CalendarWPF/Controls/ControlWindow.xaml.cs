using CalendarWPF.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CalendarWPF.Controls
{
    /// <summary>
    /// ControlWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ControlWindow : Window
    {
        private MainWindow mainWindow;

        public ControlWindow()
        {
            InitializeComponent();
        }

        public ControlWindow(MainWindow mainWindow)
            : this()
        {
            this.mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFonts();
        }

        private void LoadFonts()
        {
            var fonts = Fonts.SystemFontFamilies;
            foreach (FontFamily font in fonts)
            {
                ComboBoxItem item = new ComboBoxItem();
                string fontFamilyName = null;
                item.FontSize = 16;
                if (font.FamilyNames.ContainsKey(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name))) // 현재 설정되어있는 나라 값에 맞는 폰트를 불러옵니다. 
                {
                    fontFamilyName = font.FamilyNames[XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name)];
                }
                else // 현재 언어 값을 포함하고 있지 않다면 기본 설정을 가져온다.
                {
                    fontFamilyName = font.ToString();
                }

                if (fontFamilyName != null)
                {
                    item.Content = fontFamilyName;
                    item.FontFamily = new FontFamily(fontFamilyName);
                    ComboBox_Font.Items.Add(item);
                }
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            PostFontInfomation();
        }

        /// <summary>
        /// mainWindow로 현재 폰트 값을 보냅니다.
        /// </summary>
        private void PostFontInfomation()
        {
            FontInformation fontInfomation = new FontInformation()
            {
                FontFamilyName = ComboBox_Font.Text,
                FontSize = TextBox_FontSize.Text
            };
            mainWindow.SetMemosFont(fontInfomation);
        }

        /// <summary>
        /// 숫자만 받을 수 있도록 처리합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_FontSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text); // 숫자일 경우 true를 반환하여 이벤트를 종료시킵니다.
        }
    }
}
