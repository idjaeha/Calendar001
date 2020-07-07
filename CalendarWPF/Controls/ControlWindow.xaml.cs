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
            AddFontsToComboBox();
            LoadSetting();
        }

        /// <summary>
        /// 세팅을 불러와 적용시킵니다.
        /// </summary>
        private void LoadSetting()
        {
            ComboBox_Font.Text = mainWindow.CurrentSetting.FontFamilyName;
            TextBox_FontSize.Text = mainWindow.CurrentSetting.FontSize;
            TextBlock_Background.Text = mainWindow.CurrentSetting.Background.ToString();
            ColorPicker_Background.SelectedColor = ColorConverter.ConvertFromString(mainWindow.CurrentSetting.Background.ToString()) as Color?;
        }


        /// <summary>
        /// 기본 폰트를 불러와 ComboBox_Font에 추가합니다.
        /// </summary>
        private void AddFontsToComboBox()
        {
            var fonts = Fonts.SystemFontFamilies;
            foreach (System.Windows.Media.FontFamily font in fonts)
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
                    item.FontFamily = new System.Windows.Media.FontFamily(fontFamilyName);
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
            // 비어있지 않다면 변경한다.
            if (!(ComboBox_Font.Text == ""))
            { 
                mainWindow.CurrentSetting.FontFamilyName = ComboBox_Font.Text;
            }
            if (!(TextBox_FontSize.Text == ""))
            {
                mainWindow.CurrentSetting.FontSize = TextBox_FontSize.Text;
            }
            if (!(TextBlock_Background.Text == ""))
            {
                mainWindow.CurrentSetting.Background = new SolidColorBrush(ColorPicker_Background.SelectedColor.GetValueOrDefault());
            }

            // Setting 값을 토대로 변경을 시도한다.
            mainWindow.SetMemosFont();
            mainWindow.SetBackground();
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

        private void ColorPicker_background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            TextBlock_Background.Text = ColorPicker_Background.SelectedColorText;
        }
    }
}
