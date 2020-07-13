using CalendarWPF.Controller;
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
        private TextDecorationCollection textDecorations;

        public ControlWindow()
        {
            InitializeComponent();
            textDecorations = new TextDecorationCollection();
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
            ComboBox_Font.Text = SettingManager.CurrentSetting.FontFamilyName;
            TextBox_FontSize.Text = SettingManager.CurrentSetting.FontSize;
            ColorPicker_Background.SelectedColor = ColorConverter.ConvertFromString(SettingManager.CurrentSetting.Background) as Color?;
            ColorPicker_Background.SelectedColor = ColorConverter.ConvertFromString(SettingManager.CurrentSetting.Background) as Color?;
            ColorPicker_OptionForeground.SelectedColor = ColorConverter.ConvertFromString(SettingManager.CurrentSetting.OptionForeground) as Color?;
            ColorPicker_MemoForeground.SelectedColor = ColorConverter.ConvertFromString(SettingManager.CurrentSetting.MemoForeground) as Color?;
            CheckBox_Bold.IsChecked = SettingManager.CurrentSetting.FontWeight == FontWeights.Normal.ToString() ? false : true;
            CheckBox_Italic.IsChecked = SettingManager.CurrentSetting.FontStyle == FontStyles.Normal.ToString() ? false : true;
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
            SaveSetting();
        }

        /// <summary>
        /// 현재 Setting 값을 저장합니다.
        /// </summary>
        private void SaveSetting()
        {
            SettingManager.SaveCurrentSetting();
            this.Close();
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
            if (mainWindow == null)
            {
                return;
            }

            if (ColorPicker_Background.SelectedColor != null)
            {
                SettingManager.CurrentSetting.Background = (new SolidColorBrush(ColorPicker_Background.SelectedColor.GetValueOrDefault())).ToString();
            }

            mainWindow.SetBackground();
        }

        private void ComboBox_Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeFont(() =>
            {
                // 비어있지 않다면 변경한다.
                if (ComboBox_Font.SelectedValue != null)
                {
                    SettingManager.CurrentSetting.FontFamilyName = ((ComboBoxItem)ComboBox_Font.SelectedValue).Content.ToString();
                }
            });
        }

        private void TextBox_FontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangeFont(() =>
            {
                if (!(TextBox_FontSize.Text == ""))
                {
                    SettingManager.CurrentSetting.FontSize = TextBox_FontSize.Text;
                }
            });
        }

        private void ColorPicker_Foreground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (mainWindow == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                SettingManager.CurrentSetting.MemoForeground = new SolidColorBrush(e.NewValue.GetValueOrDefault()).ToString();
            }

            mainWindow.SetMemoForeground();
        }

        private void ColorPicker_OptionForeground_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (mainWindow == null)
            {
                return;
            }

            if (e.NewValue != null)
            {
                SettingManager.CurrentSetting.OptionForeground = new SolidColorBrush(e.NewValue.GetValueOrDefault()).ToString();
            }

            mainWindow.SetOptionForeground();
        }

        private void CheckBox_Bold_Checked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.FontWeight = FontWeights.Bold.ToString();
            });
        }

        private void CheckBox_Bold_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.FontWeight = FontWeights.Normal.ToString();
            });
        }

        private void CheckBox_Italic_Checked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.FontStyle = FontStyles.Italic.ToString();
            });
        }

        private void CheckBox_Italic_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.FontStyle = FontStyles.Normal.ToString();
            });
        }

        private void CheckBox_UnderLine_Checked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.SetTextDecorations(TextDecorations.Underline);
            });
        }

        private void CheckBox_UnderLine_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeFont(() =>
            {
                SettingManager.CurrentSetting.SetTextDecorations(null);
            });
        }

        /// <summary>
        /// 매개변수가 없는 Action을 인자로 받아 폰트 값을 변경하게 합니다.
        /// </summary>
        /// <param name="changeAction"></param>
        private void ChangeFont(Action changeAction)
        {
            if (mainWindow == null)
            {
                return;
            }
            changeAction();
            mainWindow.SetMemosFont();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SaveSetting();
        }

        private void ControlWindow_Main_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
